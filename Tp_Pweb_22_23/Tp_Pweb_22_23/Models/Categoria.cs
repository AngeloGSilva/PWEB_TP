using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Tp_Pweb_22_23.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        [Display(Name = "Categoria")]
        public string Nome { get; set; }
        public ICollection<Veiculo>? Veiculos { get; set; }
    }
}
