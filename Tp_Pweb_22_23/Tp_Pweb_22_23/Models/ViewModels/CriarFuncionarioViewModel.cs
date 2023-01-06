using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Tp_Pweb_22_23.Models.ViewModels
{
    public class CriarFuncionarioViewModel
    {
        public string Email { get; set; }
        [Display(Name = "Nome")]
        public string PrimeiroNome { get; set; }
        [Display(Name = "Apelido")]
        public string UltimoNome { get; set; }
        public string Password { get; set; }
        [Display(Name = "Ativo")]
        public bool Activo { get; set; }
        public string Role { get; set; }
    }
}
