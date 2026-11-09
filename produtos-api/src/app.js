require('dotenv').config();
const path = require('path');
const express = require('express');
const { PrismaClient, Prisma } = require('@prisma/client');

// Decimal do Prisma serializa como string; devolve como numero no JSON.
Prisma.Decimal.prototype.toJSON = function () {
  return Number(this);
};

const prisma = new PrismaClient();
const app = express();
app.use(express.json());
// Front-end (paginas HTML/CSS/JS) servido pela propria API: mesma origem, sem CORS.
app.use(express.static(path.join(__dirname, '..', 'public')));

const CAMPOS = ['nome', 'codigo', 'preco', 'descricao', 'quantidadeEstoque', 'avaliacao', 'categoria'];

// Valida o corpo da requisicao. Em atualizacoes parciais (PATCH) so valida o que veio.
function validar(body, parcial = false) {
  const erros = [];
  const d = {};
  const tem = (c) => body[c] !== undefined && body[c] !== null && body[c] !== '';

  for (const c of CAMPOS) {
    if (!tem(c)) {
      if (!parcial) erros.push(`'${c}' e obrigatorio`);
      continue;
    }
    d[c] = body[c];
  }
  for (const c of ['nome', 'codigo', 'descricao', 'categoria']) {
    if (c in d) d[c] = String(d[c]).trim();
  }
  if ('preco' in d) {
    d.preco = Number(d.preco);
    if (!Number.isFinite(d.preco) || d.preco < 0) erros.push("'preco' deve ser um numero >= 0");
  }
  if ('quantidadeEstoque' in d) {
    d.quantidadeEstoque = Number(d.quantidadeEstoque);
    if (!Number.isInteger(d.quantidadeEstoque) || d.quantidadeEstoque < 0)
      erros.push("'quantidadeEstoque' deve ser um inteiro >= 0");
  }
  if ('avaliacao' in d) {
    d.avaliacao = Number(d.avaliacao);
    if (!Number.isFinite(d.avaliacao) || d.avaliacao < 0 || d.avaliacao > 5)
      erros.push("'avaliacao' deve ser um numero entre 0 e 5");
  }
  // 'imagem' (URL) e opcional; string vazia limpa o campo.
  if (body.imagem !== undefined) {
    const img = body.imagem === null ? '' : String(body.imagem).trim();
    if (img.length > 500) erros.push("'imagem' deve ter no maximo 500 caracteres");
    d.imagem = img || null;
  }
  return { erros, dados: d };
}

// Trata erros do Prisma e devolve status HTTP adequado.
function tratarErro(res, e) {
  if (e instanceof Prisma.PrismaClientKnownRequestError) {
    if (e.code === 'P2002') return res.status(409).json({ erro: 'Ja existe um produto com esse codigo' });
    if (e.code === 'P2025') return res.status(404).json({ erro: 'Produto nao encontrado' });
  }
  console.error(e);
  return res.status(500).json({ erro: 'Erro interno do servidor' });
}

app.get('/', (req, res) =>
  res.json({
    api: 'API de Produtos',
    rotas: [
      'POST   /produtos',
      'GET    /produtos',
      'GET    /produtos/:codigo',
      'GET    /produtos/categoria/:categoria',
      'PUT    /produtos/:codigo',
      'PATCH  /produtos/:codigo',
      'DELETE /produtos/:codigo',
    ],
  })
);

// CREATE
app.post('/produtos', async (req, res) => {
  const { erros, dados } = validar(req.body);
  if (erros.length) return res.status(400).json({ erros });
  try {
    res.status(201).json(await prisma.produto.create({ data: dados }));
  } catch (e) {
    tratarErro(res, e);
  }
});

// READ - todos (filtros opcionais: ?categoria=X e ?ultimos=N, os N mais recentes primeiro)
app.get('/produtos', async (req, res) => {
  try {
    const where = req.query.categoria ? { categoria: String(req.query.categoria) } : {};
    const n = parseInt(req.query.ultimos, 10);
    if (n > 0) {
      return res.json(await prisma.produto.findMany({ where, orderBy: { id: 'desc' }, take: Math.min(n, 100) }));
    }
    res.json(await prisma.produto.findMany({ where, orderBy: { id: 'asc' } }));
  } catch (e) {
    tratarErro(res, e);
  }
});

// READ - por categoria (declarada antes de /:codigo para nao ser capturada por ela)
app.get('/produtos/categoria/:categoria', async (req, res) => {
  try {
    const lista = await prisma.produto.findMany({
      where: { categoria: req.params.categoria },
      orderBy: { id: 'asc' },
    });
    if (!lista.length) return res.status(404).json({ erro: 'Nenhum produto nessa categoria' });
    res.json(lista);
  } catch (e) {
    tratarErro(res, e);
  }
});

// READ - por codigo do produto
app.get('/produtos/:codigo', async (req, res) => {
  try {
    const p = await prisma.produto.findUnique({ where: { codigo: req.params.codigo } });
    if (!p) return res.status(404).json({ erro: 'Produto nao encontrado' });
    res.json(p);
  } catch (e) {
    tratarErro(res, e);
  }
});

// UPDATE - PUT substitui todos os campos; PATCH altera apenas os enviados
app.put('/produtos/:codigo', async (req, res) => {
  const { erros, dados } = validar(req.body);
  if (erros.length) return res.status(400).json({ erros });
  try {
    res.json(await prisma.produto.update({ where: { codigo: req.params.codigo }, data: dados }));
  } catch (e) {
    tratarErro(res, e);
  }
});

app.patch('/produtos/:codigo', async (req, res) => {
  const { erros, dados } = validar(req.body, true);
  if (erros.length) return res.status(400).json({ erros });
  if (!Object.keys(dados).length) return res.status(400).json({ erro: 'Nenhum campo para atualizar' });
  try {
    res.json(await prisma.produto.update({ where: { codigo: req.params.codigo }, data: dados }));
  } catch (e) {
    tratarErro(res, e);
  }
});

// DELETE
app.delete('/produtos/:codigo', async (req, res) => {
  try {
    const removido = await prisma.produto.delete({ where: { codigo: req.params.codigo } });
    res.json({ mensagem: 'Produto removido', produto: removido });
  } catch (e) {
    tratarErro(res, e);
  }
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`API de produtos em http://localhost:${PORT}`));
