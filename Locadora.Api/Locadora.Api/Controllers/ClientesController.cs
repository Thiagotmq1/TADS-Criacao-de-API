using Locadora.Api.Data;
using Locadora.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly ApplicationContext _context;
    public ClientesController(ApplicationContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetAll() =>
        await _context.Clientes.AsNoTracking().ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Cliente>> GetById(int id)
    {
        var item = await _context.Clientes.FindAsync(id);
        if (item is null) return NotFound(new { mensagem = $"Cliente {id} não encontrado." });
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<Cliente>> Create(Cliente cliente)
    {
        cliente.Id = 0;
        var erro = await ValidarRegras(cliente);
        if (erro is not null) return erro;
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Cliente cliente)
    {
        if (id != cliente.Id) return BadRequest(new { mensagem = "O id da URL difere do id do corpo." });
        if (!await _context.Clientes.AnyAsync(x => x.Id == id))
            return NotFound(new { mensagem = $"Cliente {id} não encontrado." });
        var erro = await ValidarRegras(cliente);
        if (erro is not null) return erro;
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Clientes.FindAsync(id);
        if (item is null) return NotFound(new { mensagem = $"Cliente {id} não encontrado." });
        if (await _context.Alugueis.AnyAsync(a => a.ClienteId == id))
            return Conflict(new { mensagem = "Cliente possui aluguéis e não pode ser removido." });
        _context.Clientes.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<ActionResult?> ValidarRegras(Cliente c)
    {
        if (await _context.Clientes.AnyAsync(x => x.Cpf == c.Cpf && x.Id != c.Id))
            return Conflict(new { mensagem = "Já existe cliente com esse CPF." });
        return null;
    }
}