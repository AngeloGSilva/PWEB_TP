namespace Tp_Pweb_22_23.Models
{
    public class Veiculo
    {
        public int Id { get; set; }
        public string Marca { get; set; }
        public bool Disponivel { get; set; }
        public bool Danos { get; set; }
        public string? Condicao { get; set; }
        public string Localizacao { get; set; }
        public decimal Preco { get; set; }
        public int? idEmpresa { get; set; }
        public Empresa? Empresa { get; set; }
        public int? idCategoria { get; set; }
        public Categoria? Categoria { get; set; }
        ICollection<Reserva> reservas { get; set; }
    }
}
