# API de Produtos (Node.js + Express + Prisma + SQL Server Express)

CRUD de produtos. O Prisma faz o papel do Entity Framework: o modelo fica em `prisma/schema.prisma` e `prisma db push` cria o banco e a tabela.

## Pre-requisitos
- Node.js 18+
- SQL Server Express com **TCP/IP habilitado na porta 1433** (SQL Server Configuration Manager > Protocolos do SQLEXPRESS > TCP/IP > Habilitado; IPAll: porta TCP 1433, portas dinamicas vazio; reiniciar o servico).

## Executar
```
npm install
copy .env.example .env      # ajuste a DATABASE_URL se necessario
npm run db:push             # cria o banco ProdutosDB e a tabela Produtos
npm run db:seed             # insere registros iniciais (edite seu codigo em prisma/seed.js)
npm start                   # http://localhost:3000
```

## Rotas
| Metodo | Rota | Descricao |
|---|---|---|
| POST | /produtos | Cria produto |
| GET | /produtos | Lista todos |
| GET | /produtos/:codigo | Busca por codigo do produto |
| GET | /produtos/categoria/:categoria | Lista por categoria |
| PUT | /produtos/:codigo | Atualiza todos os campos |
| PATCH | /produtos/:codigo | Atualiza apenas os campos enviados |
| DELETE | /produtos/:codigo | Remove |

Corpo de exemplo:
```json
{
  "nome": "Mouse Logitech", "codigo": "MS-010", "preco": 129.9,
  "descricao": "Mouse sem fio", "quantidadeEstoque": 30,
  "avaliacao": 4.6, "categoria": "Perifericos"
}
```
`GET /produtos?ultimos=12` devolve os 12 produtos mais recentes. O campo opcional `imagem` guarda a URL da foto.

## Front-end (Bootstrap + fetch)
Servido pela propria API: com `npm start`, abra http://localhost:3000
- `/` (index.html): destaque (carrossel), ultimos 12 produtos com imagem e preco, botao "Ver detalhes" e modal "Novo produto".
- `/detalhes.html?codigo=XXX`: todos os dados do produto, modal "Editar" e botao "Excluir" com confirmacao.
Arquivos em `public/` (`js/common.js` concentra os `fetch`).

A colecao do Postman esta em `postman/Produtos-API.postman_collection.json`.
