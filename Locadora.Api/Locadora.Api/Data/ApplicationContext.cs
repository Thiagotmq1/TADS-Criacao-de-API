using Locadora.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Fabricante> Fabricantes { get; set; }
    public DbSet<Veiculo> Veiculos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Aluguel> Alugueis { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Cliente>().HasIndex(c => c.Cpf).IsUnique();
        mb.Entity<Veiculo>().HasIndex(v => v.Placa).IsUnique();

        mb.Entity<Veiculo>().HasOne(v => v.Fabricante).WithMany(f => f.Veiculos)
            .HasForeignKey(v => v.FabricanteId).OnDelete(DeleteBehavior.Restrict);
        mb.Entity<Veiculo>().HasOne(v => v.Categoria).WithMany(c => c.Veiculos)
            .HasForeignKey(v => v.CategoriaId).OnDelete(DeleteBehavior.Restrict);
        mb.Entity<Aluguel>().HasOne(a => a.Cliente).WithMany(c => c.Alugueis)
            .HasForeignKey(a => a.ClienteId).OnDelete(DeleteBehavior.Restrict);
        mb.Entity<Aluguel>().HasOne(a => a.Veiculo).WithMany(v => v.Alugueis)
            .HasForeignKey(a => a.VeiculoId).OnDelete(DeleteBehavior.Restrict);
    }
}