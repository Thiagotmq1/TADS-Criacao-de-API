montarLayout();

const lista = document.getElementById('lista');
const mensagem = document.getElementById('mensagem');

function cardHtml(p) {
  return `
    <div class="col-6 col-md-4 col-lg-3">
      <div class="card card-produto h-100">
        ${imgTag(p, 'card-img-top')}
        <div class="card-body d-flex flex-column">
          <div class="nome mb-2">${esc(p.nome)}</div>
          <div class="preco mb-3">${brl(p.preco)}</div>
          <a class="btn btn-outline-primary btn-sm mt-auto" href="detalhes.html?codigo=${encodeURIComponent(p.codigo)}">Ver detalhes</a>
        </div>
      </div>
    </div>`;
}

function renderCarrossel(produtos) {
  const destaques = produtos.slice(0, 3);
  document.getElementById('carrosselItens').innerHTML = destaques
    .map(
      (p, i) => `
      <div class="carousel-item ${i === 0 ? 'active' : ''}">
        <a href="detalhes.html?codigo=${encodeURIComponent(p.codigo)}">${imgTag(p, 'd-block w-100')}</a>
        <div class="carousel-caption"><h5 class="mb-0">${esc(p.nome)} - ${brl(p.preco)}</h5></div>
      </div>`
    )
    .join('');
  document.getElementById('carrosselIndicadores').innerHTML = destaques
    .map((_, i) => `<button type="button" data-bs-target="#carrossel" data-bs-slide-to="${i}" class="${i === 0 ? 'active' : ''}"></button>`)
    .join('');
}

async function carregar() {
  try {
    const produtos = await Api.ultimos(12);
    if (!produtos.length) {
      mensagem.innerHTML = '<div class="alert alert-info">Nenhum produto cadastrado. Clique em "Novo produto".</div>';
      lista.innerHTML = '';
      document.querySelector('.destaque').classList.add('d-none');
      return;
    }
    mensagem.innerHTML = '';
    document.querySelector('.destaque').classList.remove('d-none');
    renderCarrossel(produtos);
    lista.innerHTML = produtos.map(cardHtml).join('');
  } catch (err) {
    mensagem.innerHTML = `<div class="alert alert-danger">Não foi possível carregar os produtos: ${esc(err.message)}</div>`;
  }
}

const modalProduto = criarModalProduto(async (dados) => {
  await Api.criar(dados);
  await carregar();
});
document.getElementById('btnNovo').addEventListener('click', () => modalProduto.abrir());

carregar();
