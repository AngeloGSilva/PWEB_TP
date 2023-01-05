using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Tp_Pweb_22_23.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        [Display(Name = "Empresa")]
        public string Nome {get; set; }
        public decimal SomaClassificacoes { get; set; } = 0;
        public int NrClassificacoes { get; set; } = 0;
        [Display(Name = "Classificação")]
        public decimal Classificacao { get; set; } = 0;
        public bool Ativo { get; set; } = true;
        public ICollection<Veiculo>? Veiculos { get; set; }
        public ICollection<ApplicationUser>? Funcionarios { get; set; }

    }
}
