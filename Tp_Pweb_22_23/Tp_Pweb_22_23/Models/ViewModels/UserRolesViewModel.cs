using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Tp_Pweb_22_23.Models.ViewModels
{
    public class UserRolesViewModel
    {
        public byte[]? Avatar { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Nome")]
        public string PrimeiroNome { get; set; }
        [Display(Name = "Apelido")]

        public string UltimoNome { get; set; }
        public string UserName { get; set; }
        [Display(Name = "Empresa")]
        public string EmpresaNome { get; set; }
        [Display(Name = "Ativo")]
        public bool Activo { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
