using Microsoft.AspNetCore.Identity;

namespace Tp_Pweb_22_23.Models
{
    public class ApplicationUser : IdentityUser
    {
        public byte[]? Avatar { get; set; }
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
        public int NIF { get; set; }
        public int? EmpresaId { get; set; }
        public Empresa Empresa { get; set; }
        public ICollection<Reserva> Reservas{ get; set; }
    }
}
