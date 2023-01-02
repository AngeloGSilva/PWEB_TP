using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Tp_Pweb_22_23.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        [Display(Name = "Empresa:")]
        public string Nome {get; set; }
        public int Classificacao { get; set; }
        public bool Ativo { get; set; }
        public ICollection<Veiculo>? Veiculos { get; set; }
        public ICollection<ApplicationUser>? Funcionarios { get; set; }

    }
}
