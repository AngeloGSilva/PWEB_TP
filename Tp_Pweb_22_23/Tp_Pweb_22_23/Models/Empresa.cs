namespace Tp_Pweb_22_23.Models
{
    public class Empresa
    {

        public int Id { get; set; }
        public string Nome {get; set; }
        public int Classificacao { get; set; }
        public bool Ativo { get; set; }
        public ICollection<Veiculo> Veiculos { get; set; }
        public ICollection<Reserva> Reservas { get; set; }
        public ICollection<ApplicationUser> Utilizadores { get; set; }

    }
}
