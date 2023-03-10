using System.ComponentModel.DataAnnotations;

namespace Tp_Pweb_22_23.Models
{
    public class Veiculo
    {
        public byte[]? Foto { get; set; }
        public int Id { get; set; }
        public string Marca { get; set; }
        public bool Disponivel { get; set; } = true;
        public string Modelo { get; set; }
        [Display(Name = "Localização")]
        public string Localizacao { get; set; }

        [Display(Name = "Preço")]
        public decimal Preco { get; set; }
        public int? idEmpresa { get; set; }
        public Empresa? Empresa { get; set; }

        [Display(Name = "Categoria")]
        public int? idCategoria { get; set; }
        public Categoria? Categoria { get; set; }
        ICollection<Reserva> reservas { get; set; }
    }
}

