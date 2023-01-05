using System.ComponentModel.DataAnnotations;

namespace Tp_Pweb_22_23.Models
{
    public enum ESTADO{
        Pendente,Recolher, Entregar, Concluida, Cancelada, Classificar
    }

    public class Reserva
    {
        public int Id { get; set; }
        [Display(Name = "Estado da Reserva")]
        public ESTADO Estado { get; set; } = ESTADO.Pendente;
        [Display(Name = "Data de recolha")]
        [DataType(DataType.Date)]
        public DateTime DataRecolha { get; set; }
        [Display(Name = "Data de entrega")]
        [DataType(DataType.Date)]
        public DateTime DataEntrega { get; set; }
        public decimal? Total { get; set; }
        public int? VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }
        public string ClienteId { get; set; }
        public ApplicationUser Cliente { get; set; }
        public ICollection<EstadoVeiculo> estadoVeiculos { get; set; }
    }
}
