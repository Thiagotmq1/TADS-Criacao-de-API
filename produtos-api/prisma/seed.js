const { PrismaClient } = require('@prisma/client');
const prisma = new PrismaClient();

// Codigo de pessoa real do aluno
const MEU_CODIGO = '854377';

const img = (seed) => `https://picsum.photos/seed/${seed}/600/450`;

const produtos = [
  ['Produto do Aluno', MEU_CODIGO, 99.9, 'Registro com o codigo de pessoa real do aluno', 10, 4.5, 'Eletronicos'],
  ['Notebook Dell Inspiron', 'NB-001', 3499.9, 'Notebook 15 polegadas, 16GB RAM, SSD 512GB', 8, 4.7, 'Eletronicos'],
  ['Cadeira Gamer', 'MV-002', 899.0, 'Cadeira ergonomica reclinavel', 15, 4.2, 'Moveis'],
  ['Smartphone Galaxy', 'EL-003', 2199.0, 'Smartphone 128GB, camera tripla', 25, 4.6, 'Eletronicos'],
  ['Fone Bluetooth', 'EL-004', 249.9, 'Fone sem fio com cancelamento de ruido', 40, 4.3, 'Eletronicos'],
  ['Mesa de Escritorio', 'MV-005', 650.0, 'Mesa 120cm com gaveta', 12, 4.1, 'Moveis'],
  ['Monitor 24 pol', 'EL-006', 799.9, 'Monitor Full HD IPS 75Hz', 18, 4.4, 'Eletronicos'],
  ['Teclado Mecanico', 'PF-007', 289.9, 'Teclado ABNT2 switch azul', 30, 4.5, 'Perifericos'],
  ['Mouse Gamer', 'PF-008', 159.9, 'Mouse 6 botoes 7200 DPI', 35, 4.2, 'Perifericos'],
  ['Webcam Full HD', 'PF-009', 219.0, 'Webcam 1080p com microfone', 22, 4.0, 'Perifericos'],
  ['Estante de Livros', 'MV-010', 420.0, 'Estante 5 prateleiras', 9, 4.3, 'Moveis'],
  ['Luminaria de Mesa', 'MV-011', 89.9, 'Luminaria LED articulada', 50, 4.6, 'Moveis'],
  ['Caixa de Som Portatil', 'EL-012', 329.0, 'Caixa de som bluetooth a prova d\'agua', 16, 4.4, 'Eletronicos'],
  ['Hub USB-C', 'PF-013', 139.9, 'Hub 7 em 1 com HDMI', 28, 4.1, 'Perifericos'],
].map(([nome, codigo, preco, descricao, quantidadeEstoque, avaliacao, categoria]) => ({
  nome, codigo, preco, descricao, quantidadeEstoque, avaliacao, categoria, imagem: img(codigo),
}));

async function main() {
  for (const p of produtos) {
    await prisma.produto.upsert({ where: { codigo: p.codigo }, update: p, create: p });
  }
  console.log(`${produtos.length} produtos inseridos/atualizados.`);
}

main().finally(() => prisma.$disconnect());
