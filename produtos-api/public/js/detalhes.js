montarLayout();

const codigo = new URLSearchParams(location.search).get('codigo');
const detalhe = document.getElementById('detalhe');
const mensagem = document.getElementById('mensagem');
let produto = null;

const estrelas = (n) => {
  const cheias = Math.round(Number(n));
  return '★'.repeat(cheias) + '☆'.repeat(5 - cheias);
};

function render(p) {
  document.title = `${p.nome} - Detalhes`;
  detalhe.innerHTML = `
    <div class="col-md-5">${imgTag(p, 'detalhe-img')}</div>
    <div class="col-md-7">
      <h1 class="h3">${esc(p.nome)}</h1>
      <div class="preco fs-3 mb-2">${brl(p.preco)}</div>
      <p class="estrelas mb-1">${estrelas(p.avaliacao)} <span class="text-muted">(${Number(p.avaliacao).toFixed(1)})</span></p>
      <h6 class="text-primary mt-3">Descrição</h6>
      <p>${esc(p.descricao)}</p>
      <table class="table table-sm w-auto">
        <tr><th class="pe-4">Código</th><td>${esc(p.codigo)}</td></tr>
        <tr><th>Categoria</th><td>${esc(p.categoria)}</td></tr>
        <tr><th>Estoque</th><td>${p.quantidadeEstoque} unidade(s)</td></tr>
        <tr><th>Cadastrado em</th><td>${new Date(p.criadoEm).toLocaleString('pt-BR')}</td></tr>
      </table>
      <div class="d-flex gap-2">
        <button class="btn btn-primary" id="btnEditar"><i class="bi bi-pencil"></i> Editar</button>
        <button class="btn btn-danger" id="btnExcluir"><i class="bi bi-trash"></i> Excluir</button>
      </div>
    </div>`;
  document.getElementById('btnEditar').addEventListener('click', () => modalProduto.abrir(produto));
  document.getElementById('btnExcluir').addEventListener('click', () => {
    document.getElementById('excluirNome').textContent = produto.nome;
    document.getElementById('excluirErro').classList.add('d-none');
    modalExcluir.show();
  });
}

const modalProduto = criarModalProduto(async (dados, codigoOriginal) => {
  produto = await Api.atualizar(codigoOriginal, dados);
  render(produto);
});

const modalExcluir = new bootstrap.Modal(document.getElementById('modalExcluir'));
document.getElementById('btnConfirmarExcluir').addEventListener('click', async () => {
  try {
    await Api.excluir(produto.codigo);
    location.href = 'index.html';
  } catch (err) {
    const e = document.getElementById('excluirErro');
    e.textContent = err.message;
    e.classList.remove('d-none');
  }
});

(async () => {
  if (!codigo) {
    mensagem.innerHTML = '<div class="alert alert-warning">Nenhum produto informado.</div>';
    return;
  }
  try {
    produto = await Api.buscar(codigo);
    render(produto);
  } catch (err) {
    mensagem.innerHTML = `<div class="alert alert-danger">${esc(err.message)}</div>`;
  }
})();
