using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Locadora.Api.Models;

public class Fabricante
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Nome { get; set; } = string.Empty;
    [StringLength(60)] public string? PaisOrigem { get; set; }
    [JsonIgnore] public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}