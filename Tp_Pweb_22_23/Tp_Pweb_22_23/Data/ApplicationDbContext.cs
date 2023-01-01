using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tp_Pweb_22_23.Models;

namespace Tp_Pweb_22_23.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Veiculo> Veiculo { get; set; }
        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<Reserva> Reserva { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<EstadoVeiculo> EstadoVeiculo { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}