namespace Tp_Pweb_22_23.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public ICollection<Veiculo> Veiculos { get; set; }
    }
}
