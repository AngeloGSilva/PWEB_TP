using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tp_Pweb_22_23.Data;
using Tp_Pweb_22_23.Models;

namespace Tp_Pweb_22_23.Controllers
{
    public class EstadoVeiculosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EstadoVeiculosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private ApplicationUser GetCurrentUser()
        {
            var user = _context.Users
                .Where(u => u.UserName == User.Identity.Name)
                .Include(u => u.Empresa)
                .FirstOrDefault();
            return user;
        }

        // GET: EstadoVeiculos
        public async Task<IActionResult> Index()
        {
            var funcionario = GetCurrentUser();
            var estados = await _context.EstadoVeiculo.ToListAsync();
            var estadosVeiculosEmpresa = new List<EstadoVeiculo>();
            foreach (var estado in estados) 
            {
                var idEmpresa = _context.Reserva.Where(r => r.Id == estado.ReservaId && r.VeiculoId != null)
                        .Select(r => r.Veiculo.idEmpresa)
                        .FirstOrDefault();

                if (funcionario.EmpresaId == idEmpresa) 
                {
                    estadosVeiculosEmpresa.Add(estado);
                }
            }

            return View(estadosVeiculosEmpresa);
        }

        // GET: EstadoVeiculos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EstadoVeiculo == null)
            {
                return NotFound();
            }

            var estadoVeiculo = await _context.EstadoVeiculo
                .Include(e => e.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estadoVeiculo == null)
            {
                return NotFound();
            }

            return View(estadoVeiculo);
        }

        // GET: EstadoVeiculos/Create
        public IActionResult Create(string FuncionarioId, int ReservaId, ESTADO EstadoReserva)
        {
            var reserva = _context.Reserva.Where(c => c.Id == ReservaId).FirstOrDefault();
            var funcionario = _context.Users.Where(c => c.Id == FuncionarioId).FirstOrDefault();

            ViewData["ReservaId"] = ReservaId;
            ViewData["EmpregadoEmail"] = funcionario.Email;
            ViewData["EmpregadoId"] = FuncionarioId;
            ViewData["EstadoReserva"] = EstadoReserva;
            return View();
        }

        // POST: EstadoVeiculos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumeroKm,Danos,Observacoes,FuncionarioId,ReservaId,ESTADO")] EstadoVeiculo estadoVeiculo)
        {
            var reserva = _context.Reserva.Where(c=> c.Id == estadoVeiculo.ReservaId).FirstOrDefault();
            var funcionario = await _userManager.FindByIdAsync(estadoVeiculo.FuncionarioId);
            estadoVeiculo.Reserva = reserva;
            estadoVeiculo.Funcionario = funcionario;
            if (ModelState.IsValid)
            {
                if (reserva.Estado == ESTADO.Entregar)
                {
                    reserva.Estado = ESTADO.Concluida;
                } else if (reserva.Estado == ESTADO.Recolher) 
                {
                    reserva.Estado = ESTADO.Entregar;
                }
                _context.Update(reserva);
                _context.Add(estadoVeiculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["ReservaId"] = new SelectList(_context.Reserva, "Id", "Id", estadoVeiculo.ReservaId);
            return View(estadoVeiculo);
        }

        // GET: EstadoVeiculos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EstadoVeiculo == null)
            {
                return NotFound();
            }

            var estadoVeiculo = await _context.EstadoVeiculo.FindAsync(id);
            if (estadoVeiculo == null)
            {
                return NotFound();
            }
            ViewData["ReservaId"] = new SelectList(_context.Reserva, "Id", "Id", estadoVeiculo.ReservaId);
            return View(estadoVeiculo);
        }

        // POST: EstadoVeiculos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumeroKm,Danos,Observacoes,FuncionarioId,ReservaId")] EstadoVeiculo estadoVeiculo)
        {
            if (id != estadoVeiculo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estadoVeiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstadoVeiculoExists(estadoVeiculo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReservaId"] = new SelectList(_context.Reserva, "Id", "Id", estadoVeiculo.ReservaId);
            return View(estadoVeiculo);
        }

        // GET: EstadoVeiculos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EstadoVeiculo == null)
            {
                return NotFound();
            }

            var estadoVeiculo = await _context.EstadoVeiculo
                .Include(e => e.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estadoVeiculo == null)
            {
                return NotFound();
            }

            return View(estadoVeiculo);
        }

        // POST: EstadoVeiculos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EstadoVeiculo == null)
            {
                return Problem("Entity set 'ApplicationDbContext.EstadoVeiculo'  is null.");
            }
            var estadoVeiculo = await _context.EstadoVeiculo.FindAsync(id);
            if (estadoVeiculo != null)
            {
                _context.EstadoVeiculo.Remove(estadoVeiculo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstadoVeiculoExists(int id)
        {
          return _context.EstadoVeiculo.Any(e => e.Id == id);
        }
    }
}
