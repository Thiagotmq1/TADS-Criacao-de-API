using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Locadora.Api.Models;

public class Veiculo
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Modelo { get; set; } = string.Empty;
    [Required, StringLength(8)] public string Placa { get; set; } = string.Empty;
    [Range(1950, 2100)] public int AnoFabricacao { get; set; }
    [Range(0, int.MaxValue)] public int Quilometragem { get; set; }

    public int FabricanteId { get; set; }
    [JsonIgnore] public Fabricante? Fabricante { get; set; }
    public int CategoriaId { get; set; }
    [JsonIgnore] public Categoria? Categoria { get; set; }
    [JsonIgnore] public ICollection<Aluguel> Alugueis { get; set; } = new List<Aluguel>();
}