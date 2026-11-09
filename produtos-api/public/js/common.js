// Funcoes compartilhadas pelas duas paginas: acesso a API (somente fetch), layout e modal de produto.
const API = '/produtos';
const JSON_HEADERS = { 'Content-Type': 'application/json' };
const SEM_IMAGEM =
  'data:image/svg+xml;utf8,' +
  encodeURIComponent(
    '<svg xmlns="http://www.w3.org/2000/svg" width="600" height="450"><rect width="100%" height="100%" fill="#e9ecef"/>' +
      '<text x="50%" y="50%" fill="#868e96" font-family="Arial" font-size="28" text-anchor="middle">Sem imagem</text></svg>'
  );

const esc = (s) =>
  String(s ?? '').replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
const brl = (n) => Number(n).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
const imgTag = (p, cls) =>
  `<img src="${esc(p.imagem || SEM_IMAGEM)}" class="${cls}" alt="${esc(p.nome)}" onerror="this.onerror=null;this.src=SEM_IMAGEM">`;

// ---------- Acesso a API (fetch) ----------
async function http(url, options) {
  const resp = await fetch(url, options);
  let dados = null;
  try {
    dados = await resp.json();
  } catch {}
  if (!resp.ok) {
    const msg = dados && (dados.erro || (dados.erros || []).join('; '));
    throw new Error(msg || `Erro ${resp.status}`);
  }
  return dados;
}

const Api = {
  ultimos: (n) => http(`${API}?ultimos=${n}`),
  buscar: (codigo) => http(`${API}/${encodeURIComponent(codigo)}`),
  criar: (p) => http(API, { method: 'POST', headers: JSON_HEADERS, body: JSON.stringify(p) }),
  atualizar: (codigo, p) =>
    http(`${API}/${encodeURIComponent(codigo)}`, { method: 'PUT', headers: JSON_HEADERS, body: JSON.stringify(p) }),
  excluir: (codigo) => http(`${API}/${encodeURIComponent(codigo)}`, { method: 'DELETE' }),
};

// ---------- Layout (topo e creditos) ----------
function montarLayout() {
  document.getElementById('topo').innerHTML = `
    <nav class="navbar navbar-expand-sm bg-white border-bottom">
      <div class="container">
        <a class="navbar-brand" href="index.html"><img src="img/logo.svg" alt="Logotipo" height="32" width="32"></a>
        <button class="navbar-toggler" data-bs-toggle="collapse" data-bs-target="#menu"><span class="navbar-toggler-icon"></span></button>
        <div class="collapse navbar-collapse justify-content-end" id="menu">
          <ul class="navbar-nav">
            <li class="nav-item"><a class="nav-link" href="index.html">Home</a></li>
            <li class="nav-item"><a class="nav-link" href="index.html#produtos">Produtos</a></li>
            <li class="nav-item"><a class="nav-link" href="#creditos">Créditos</a></li>
          </ul>
        </div>
      </div>
    </nav>`;
  document.getElementById('creditos').innerHTML = `
    <div class="container py-4">
      <h2 class="secao">Créditos</h2>
      <div class="row">
        <div class="col-md-8">
          <h6 class="text-primary">Sobre</h6>
          <p class="text-muted mb-0">Projeto da disciplina de Criação de API: front-end em HTML, CSS, Bootstrap e JavaScript
          consumindo, somente com fetch, uma API Node.js (Express + Prisma) com persistência no SQL Server Express.</p>
        </div>
        <div class="col-md-4">
          <h6 class="text-primary">Autoria</h6>
          <div class="d-flex align-items-center gap-3">
            <i class="bi bi-person-circle fs-1"></i>
            <small>
              <b>Aluno:</b> Thiago Moreira de Queiroz<br><b>Curso:</b> Análise e Desenvolvimento de Sistemas<br><b>Turma:</b> Sua Turma
            </small>
          </div>
          <div class="mt-2"><small class="text-primary fw-bold">Redes Sociais:</small>
            <i class="bi bi-facebook mx-1"></i><i class="bi bi-twitter mx-1"></i><i class="bi bi-instagram mx-1"></i>
          </div>
        </div>
      </div>
    </div>`;
}

// ---------- Modal de cadastro/edicao ----------
// onSalvar(dados, codigoOriginal|null)
function criarModalProduto(onSalvar) {
  document.body.insertAdjacentHTML(
    'beforeend',
    `<div class="modal fade" id="modalProduto" tabindex="-1"><div class="modal-dialog modal-lg"><div class="modal-content">
      <form id="formProduto" novalidate>
        <div class="modal-header"><h5 class="modal-title" id="modalProdutoTitulo"></h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal"></button></div>
        <div class="modal-body"><div class="row g-3">
          <div class="col-md-8"><label class="form-label">Nome</label><input name="nome" class="form-control" required maxlength="150"></div>
          <div class="col-md-4"><label class="form-label">Código</label><input name="codigo" class="form-control" required maxlength="50"></div>
          <div class="col-md-4"><label class="form-label">Preço (R$)</label><input name="preco" type="number" step="0.01" min="0" class="form-control" required></div>
          <div class="col-md-4"><label class="form-label">Estoque</label><input name="quantidadeEstoque" type="number" step="1" min="0" class="form-control" required></div>
          <div class="col-md-4"><label class="form-label">Avaliação (0 a 5)</label><input name="avaliacao" type="number" step="0.1" min="0" max="5" class="form-control" required></div>
          <div class="col-md-6"><label class="form-label">Categoria</label><input name="categoria" class="form-control" required maxlength="80"></div>
          <div class="col-md-6"><label class="form-label">URL da imagem</label><input name="imagem" type="url" class="form-control" maxlength="500" placeholder="https://..."></div>
          <div class="col-12"><label class="form-label">Descrição</label><textarea name="descricao" rows="3" class="form-control" required maxlength="500"></textarea></div>
        </div><div id="formErro" class="alert alert-danger mt-3 d-none"></div></div>
        <div class="modal-footer"><button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
          <button type="submit" class="btn btn-primary" id="btnSalvar">Salvar</button></div>
      </form></div></div></div>`
  );
  const el = document.getElementById('modalProduto');
  const modal = new bootstrap.Modal(el);
  const form = document.getElementById('formProduto');
  const erro = document.getElementById('formErro');
  let codigoOriginal = null;

  function abrir(produto) {
    form.reset();
    erro.classList.add('d-none');
    codigoOriginal = produto ? produto.codigo : null;
    document.getElementById('modalProdutoTitulo').textContent = produto ? 'Editar produto' : 'Novo produto';
    form.codigo.readOnly = !!produto; // o codigo identifica o registro na API
    if (produto) for (const [k, v] of Object.entries(produto)) if (form.elements[k]) form.elements[k].value = v ?? '';
    modal.show();
  }

  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    if (!form.checkValidity()) return form.reportValidity();
    const dados = Object.fromEntries(new FormData(form).entries());
    const btn = document.getElementById('btnSalvar');
    btn.disabled = true;
    try {
      await onSalvar(dados, codigoOriginal);
      modal.hide();
    } catch (err) {
      erro.textContent = err.message;
      erro.classList.remove('d-none');
    } finally {
      btn.disabled = false;
    }
  });

  return { abrir };
}
