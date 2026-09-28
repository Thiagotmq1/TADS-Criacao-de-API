using Locadora.Api.Data;
using Locadora.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/fabricantes")]
public class FabricantesController : ControllerBase
{
    private readonly ApplicationContext _context;
    public FabricantesController(ApplicationContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Fabricante>>> GetAll() =>
        await _context.Fabricantes.AsNoTracking().ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Fabricante>> GetById(int id)
    {
        var item = await _context.Fabricantes.FindAsync(id);
        if (item is null) return NotFound(new { mensagem = $"Fabricante {id} não encontrado." });
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<Fabricante>> Create(Fabricante fabricante)
    {
        fabricante.Id = 0;
        var erro = await ValidarRegras(fabricante);
        if (erro is not null) return erro;
        _context.Fabricantes.Add(fabricante);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = fabricante.Id }, fabricante);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Fabricante fabricante)
    {
        if (id != fabricante.Id) return BadRequest(new { mensagem = "O id da URL difere do id do corpo." });
        if (!await _context.Fabricantes.AnyAsync(x => x.Id == id))
            return NotFound(new { mensagem = $"Fabricante {id} não encontrado." });
        var erro = await ValidarRegras(fabricante);
        if (erro is not null) return erro;
        _context.Fabricantes.Update(fabricante);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Fabricantes.FindAsync(id);
        if (item is null) return NotFound(new { mensagem = $"Fabricante {id} não encontrado." });
        if (await _context.Veiculos.AnyAsync(v => v.FabricanteId == id))
            return Conflict(new { mensagem = "Fabricante possui veículos e não pode ser removido." });
        _context.Fabricantes.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<ActionResult?> ValidarRegras(Fabricante f)
    {
        if (await _context.Fabricantes.AnyAsync(x => x.Nome == f.Nome && x.Id != f.Id))
            return Conflict(new { mensagem = "Já existe fabricante com esse nome." });
        return null;
    }
}