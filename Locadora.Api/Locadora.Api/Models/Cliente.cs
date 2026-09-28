using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Locadora.Api.Models;

public class Cliente
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Nome { get; set; } = string.Empty;
    [Required, RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve ter 11 dígitos numéricos.")]
    public string Cpf { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(150)] public string Email { get; set; } = string.Empty;
    [StringLength(20)] public string? Telefone { get; set; }
    [JsonIgnore] public ICollection<Aluguel> Alugueis { get; set; } = new List<Aluguel>();
}