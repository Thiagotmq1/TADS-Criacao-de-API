using Locadora.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/consultas")]
public class ConsultasController : ControllerBase
{
    private readonly ApplicationContext _context;
    public ConsultasController(ApplicationContext context) => _context = context;

    // INNER JOIN Veiculo × Fabricante
    [HttpGet("veiculos-por-fabricante")]
    public async Task<IActionResult> VeiculosPorFabricante([FromQuery] string nome) =>
        Ok(await (from v in _context.Veiculos
                  join f in _context.Fabricantes on v.FabricanteId equals f.Id
                  where f.Nome.Contains(nome)
                  select new { v.Id, v.Modelo, v.Placa, v.AnoFabricacao, Fabricante = f.Nome })
                 .ToListAsync());

    // INNER JOIN Aluguel × Cliente × Veiculo
    [HttpGet("alugueis-por-cliente/{cpf}")]
    public async Task<IActionResult> AlugueisPorCliente(string cpf) =>
        Ok(await (from a in _context.Alugueis
                  join c in _context.Clientes on a.ClienteId equals c.Id
                  join v in _context.Veiculos on a.VeiculoId equals v.Id
                  where c.Cpf == cpf
                  select new { a.Id, Cliente = c.Nome, v.Modelo, v.Placa, a.DataInicio, a.DataDevolucao, a.ValorTotal })
                 .ToListAsync());

    // LEFT JOIN Veiculo × Aluguel em aberto → veículos disponíveis
    [HttpGet("veiculos-disponiveis")]
    public async Task<IActionResult> VeiculosDisponiveis() =>
        Ok(await (from v in _context.Veiculos
                  join a in _context.Alugueis.Where(x => x.DataDevolucao == null)
                      on v.Id equals a.VeiculoId into ga
                  from a in ga.DefaultIfEmpty()
                  where a == null
                  select new { v.Id, v.Modelo, v.Placa, v.Quilometragem })
                 .ToListAsync());

    // LEFT JOIN Fabricante × Veiculo → inclui fabricantes sem veículos
    [HttpGet("fabricantes-e-veiculos")]
    public async Task<IActionResult> FabricantesEVeiculos() =>
        Ok(await (from f in _context.Fabricantes
                  join v in _context.Veiculos on f.Id equals v.FabricanteId into gv
                  from v in gv.DefaultIfEmpty()
                  select new { Fabricante = f.Nome, Modelo = v == null ? null : v.Modelo, Placa = v == null ? null : v.Placa })
                 .ToListAsync());

    // INNER JOIN Aluguel × Cliente × Veiculo × Categoria → aluguéis atrasados
    [HttpGet("alugueis-atrasados")]
    public async Task<IActionResult> AlugueisAtrasados() =>
        Ok(await (from a in _context.Alugueis
                  join c in _context.Clientes on a.ClienteId equals c.Id
                  join v in _context.Veiculos on a.VeiculoId equals v.Id
                  join cat in _context.Categorias on v.CategoriaId equals cat.Id
                  where a.DataDevolucao == null && a.DataFimPrevista < DateTime.Now
                  select new { a.Id, Cliente = c.Nome, c.Email, v.Placa, Categoria = cat.Nome, a.DataFimPrevista })
                 .ToListAsync());
}