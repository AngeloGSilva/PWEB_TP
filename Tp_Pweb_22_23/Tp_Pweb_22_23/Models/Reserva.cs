using System.ComponentModel.DataAnnotations;

namespace Tp_Pweb_22_23.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        [Display(Name = "Reserva ativa?")]
        public bool Estado { get; set; }
        [Display(Name = "Data de recolha")]
        public DateTime DataRecolha { get; set; }
        [Display(Name = "Data de entrega")]
        public DateTime DataEntrega { get; set; }
        public int? VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }
        public string ClienteId { get; set; }
        public ApplicationUser Cliente { get; set; }
        public ICollection<EstadoVeiculo> estadoVeiculos { get; set; }
    }
}
