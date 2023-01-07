using System.ComponentModel.DataAnnotations;

namespace Tp_Pweb_22_23.Models
{
    public class EstadoVeiculo
    {
        public int Id { get; set; }
        [Display(Name = "Estado")]
        public ESTADO ESTADO { get; set; }
        public int NumeroKm { get; set; }
        public bool Danos { get; set; }
        [Display(Name = "Observações ")]
        public string? Observacoes { get; set; }
        public string? FuncionarioId { get; set; }
        public ApplicationUser? Funcionario { get; set; }
        public int? ReservaId { get; set; }
        public Reserva? Reserva { get; set; }
    }
}
