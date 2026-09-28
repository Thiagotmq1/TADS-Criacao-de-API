using Locadora.Api.Data;
using Locadora.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/veiculos")]
public class VeiculoesController : ControllerBase
{
    private readonly ApplicationContext _context;
    public VeiculoesController(ApplicationContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Veiculo>>> GetAll() =>
        await _context.Veiculos.AsNoTracking().ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Veiculo>> GetById(int id)
    {
        var item = await _context.Veiculos.FindAsync(id);
        if (item is null) return NotFound(new { mensagem = $"Veículo {id} não encontrado." });
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<Veiculo>> Create(Veiculo veiculo)
    {
        veiculo.Id = 0;
        var erro = await ValidarRegras(veiculo);
        if (erro is not null) return erro;
        _context.Veiculos.Add(veiculo);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = veiculo.Id }, veiculo);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Veiculo veiculo)
    {
        if (id != veiculo.Id) return BadRequest(new { mensagem = "O id da URL difere do id do corpo." });
        if (!await _context.Veiculos.AnyAsync(x => x.Id == id))
            return NotFound(new { mensagem = $"Veículo {id} não encontrado." });
        var erro = await ValidarRegras(veiculo);
        if (erro is not null) return erro;
        _context.Veiculos.Update(veiculo);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Veiculos.FindAsync(id);
        if (item is null) return NotFound(new { mensagem = $"Veículo {id} não encontrado." });
        if (await _context.Alugueis.AnyAsync(a => a.VeiculoId == id))
            return Conflict(new { mensagem = "Veículo possui aluguéis e não pode ser removido." });
        _context.Veiculos.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<ActionResult?> ValidarRegras(Veiculo v)
    {
        if (!await _context.Fabricantes.AnyAsync(f => f.Id == v.FabricanteId))
            return BadRequest(new { mensagem = "Fabricante informado não existe." });
        if (!await _context.Categorias.AnyAsync(c => c.Id == v.CategoriaId))
            return BadRequest(new { mensagem = "Categoria informada não existe." });
        if (await _context.Veiculos.AnyAsync(x => x.Placa == v.Placa && x.Id != v.Id))
            return Conflict(new { mensagem = "Já existe veículo com essa placa." });
        return null;
    }
}