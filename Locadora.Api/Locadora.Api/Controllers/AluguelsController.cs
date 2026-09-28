using Locadora.Api.Data;
using Locadora.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/alugueis")]
public class AluguelsController : ControllerBase
{
    private readonly ApplicationContext _context;
    public AluguelsController(ApplicationContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Aluguel>>> GetAll() =>
        await _context.Alugueis.AsNoTracking().ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Aluguel>> GetById(int id)
    {
        var item = await _context.Alugueis.FindAsync(id);
        if (item is null) return NotFound(new { mensagem = $"Aluguel {id} não encontrado." });
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<Aluguel>> Create(Aluguel aluguel)
    {
        aluguel.Id = 0;
        var erro = await ValidarRegras(aluguel);
        if (erro is not null) return erro;
        _context.Alugueis.Add(aluguel);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = aluguel.Id }, aluguel);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Aluguel aluguel)
    {
        if (id != aluguel.Id) return BadRequest(new { mensagem = "O id da URL difere do id do corpo." });
        if (!await _context.Alugueis.AnyAsync(x => x.Id == id))
            return NotFound(new { mensagem = $"Aluguel {id} não encontrado." });
        var erro = await ValidarRegras(aluguel);
        if (erro is not null) return erro;
        _context.Alugueis.Update(aluguel);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Alugueis.FindAsync(id);
        if (item is null) return NotFound(new { mensagem = $"Aluguel {id} não encontrado." });
        _context.Alugueis.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<ActionResult?> ValidarRegras(Aluguel a)
    {
        if (!await _context.Clientes.AnyAsync(c => c.Id == a.ClienteId))
            return BadRequest(new { mensagem = "Cliente informado não existe." });
        if (!await _context.Veiculos.AnyAsync(v => v.Id == a.VeiculoId))
            return BadRequest(new { mensagem = "Veículo informado não existe." });
        if (a.DataFimPrevista <= a.DataInicio)
            return BadRequest(new { mensagem = "A data prevista de fim deve ser posterior ao início." });
        if (a.DataDevolucao is not null && a.DataDevolucao < a.DataInicio)
            return BadRequest(new { mensagem = "A devolução não pode ser anterior ao início." });
        if (a.KmFinal is not null && a.DataDevolucao is null)
            return BadRequest(new { mensagem = "Informe a data de devolução junto com a km final." });
        if (a.KmFinal is not null && a.KmFinal < a.KmInicial)
            return BadRequest(new { mensagem = "A km final não pode ser menor que a km inicial." });
        if (a.DataDevolucao is null && await _context.Alugueis.AnyAsync(x =>
                x.VeiculoId == a.VeiculoId && x.DataDevolucao == null && x.Id != a.Id))
            return Conflict(new { mensagem = "Este veículo já está em um aluguel em aberto." });

        if (a.DataDevolucao is not null)
        {
            var dias = Math.Max(1, (int)Math.Ceiling((a.DataDevolucao.Value - a.DataInicio).TotalDays));
            a.ValorTotal = dias * a.ValorDiaria;
        }
        return null;
    }
}