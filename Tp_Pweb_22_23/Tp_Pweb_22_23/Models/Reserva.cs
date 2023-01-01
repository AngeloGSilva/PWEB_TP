namespace Tp_Pweb_22_23.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public bool Estado { get; set; }
        public DateTime DataRecolha { get; set; }
        public DateTime DataEntrega { get; set; }
        public int? EmpresaId { get; set; }
        public Empresa? Empresa { get; set; }
        public int? VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }
        public int? ClienteId { get; set; }
        public ApplicationUser Cliente { get; set; }
        public ICollection<EstadoVeiculo> estadoVeiculos { get; set; }
    }
}
