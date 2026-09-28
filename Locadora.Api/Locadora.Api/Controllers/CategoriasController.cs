using Locadora.Api.Data;
using Locadora.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly ApplicationContext _context;
    public CategoriasController(ApplicationContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetAll() =>
        await _context.Categorias.AsNoTracking().ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Categoria>> GetById(int id)
    {
        var item = await _context.Categorias.FindAsync(id);
        if (item is null) return NotFound(new { mensagem = $"Categoria {id} não encontrada." });
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<Categoria>> Create(Categoria categoria)
    {
        categoria.Id = 0;
        var erro = await ValidarRegras(categoria);
        if (erro is not null) return erro;
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Categoria categoria)
    {
        if (id != categoria.Id) return BadRequest(new { mensagem = "O id da URL difere do id do corpo." });
        if (!await _context.Categorias.AnyAsync(x => x.Id == id))
            return NotFound(new { mensagem = $"Categoria {id} não encontrada." });
        var erro = await ValidarRegras(categoria);
        if (erro is not null) return erro;
        _context.Categorias.Update(categoria);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Categorias.FindAsync(id);
        if (item is null) return NotFound(new { mensagem = $"Categoria {id} não encontrada." });
        if (await _context.Veiculos.AnyAsync(v => v.CategoriaId == id))
            return Conflict(new { mensagem = "Categoria possui veículos e não pode ser removida." });
        _context.Categorias.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<ActionResult?> ValidarRegras(Categoria c)
    {
        if (await _context.Categorias.AnyAsync(x => x.Nome == c.Nome && x.Id != c.Id))
            return Conflict(new { mensagem = "Já existe categoria com esse nome." });
        return null;
    }
}