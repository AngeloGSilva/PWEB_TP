using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Tp_Pweb_22_23.Models
{
    public class ApplicationUser : IdentityUser
    {
        public byte[]? Avatar { get; set; }
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
        public int NIF { get; set; }
        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
        public int? EmpresaId { get; set; }
        public Empresa Empresa { get; set; }
        public ICollection<Reserva>? Reservas{ get; set; }
    }
}
