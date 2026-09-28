using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Locadora.Api.Models;

public class Aluguel
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    [JsonIgnore] public Cliente? Cliente { get; set; }
    public int VeiculoId { get; set; }
    [JsonIgnore] public Veiculo? Veiculo { get; set; }

    public DateTime DataInicio { get; set; }
    public DateTime DataFimPrevista { get; set; }
    public DateTime? DataDevolucao { get; set; }
    [Range(0, int.MaxValue)] public int KmInicial { get; set; }
    [Range(0, int.MaxValue)] public int? KmFinal { get; set; }
    [Column(TypeName = "decimal(10,2)"), Range(0.01, 100000)] public decimal ValorDiaria { get; set; }
    [Column(TypeName = "decimal(10,2)")] public decimal? ValorTotal { get; set; }
}