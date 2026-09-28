using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Locadora.Api.Models;

public class Categoria
{
    public int Id { get; set; }
    [Required, StringLength(50)] public string Nome { get; set; } = string.Empty;
    [JsonIgnore] public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}