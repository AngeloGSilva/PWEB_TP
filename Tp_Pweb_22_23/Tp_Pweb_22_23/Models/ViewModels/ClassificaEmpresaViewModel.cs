namespace Tp_Pweb_22_23.Models.ViewModels
{
    public class ClassificaEmpresaViewModel
    {
        public Empresa empresa { get; set; }
        public ApplicationUser cliente { get; set; }
        public Reserva reserva { get; set; }
        public Veiculo veiculo { get; set; }
    }
}
