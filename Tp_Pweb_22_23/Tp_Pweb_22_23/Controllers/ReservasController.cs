﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tp_Pweb_22_23.Data;
using Tp_Pweb_22_23.Models;

namespace Tp_Pweb_22_23.Controllers
{
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservasController(ApplicationDbContext context)
        {
            _context = context;
        }

        private ApplicationUser GetCurrentUser()
        {
            var user = _context.Users
                .Where(u => u.UserName == User.Identity.Name)
                .Include(u => u.Empresa)
                .FirstOrDefault();
            return user;
        }


        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var user = GetCurrentUser();

            if (User.IsInRole("Admin"))
            {
                return View(await _context.Reserva.Include(r => r.Cliente).Include(r => r.Veiculo).ToListAsync());
            }

            else if (User.IsInRole("Gestor") || User.IsInRole("Funcionario"))
            {
                var reservasComVeiculosDaMesmaEmpresa = from r in _context.Reserva
                                                        join v in _context.Veiculo on r.VeiculoId equals v.Id
                                                        where v.idEmpresa == user.EmpresaId
                                                        select r;

                return View(reservasComVeiculosDaMesmaEmpresa);
            }
            if(User.IsInRole("Cliente"))
            {
                return View(await _context.Reserva.Where(c=> c.ClienteId == user.Id).ToListAsync());
            }
            return (NotFound());
            //var applicationDbContext = _context.Reserva.Include(r => r.Cliente).Include(r => r.Veiculo);
            //return View(await applicationDbContext.ToListAsync());
        }


        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reserva == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .Include(r => r.Cliente)
                .Include(r => r.Veiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }


        public async Task<IActionResult> ConfirmReserva(int? id) 
        {
            var user = GetCurrentUser();
            var reserva = _context.Reserva.Where(c => c.Id == id).FirstOrDefault();

            var VeiculoReservaEmpresaId = (from v in _context.Veiculo
                        join r in _context.Reserva on v.Id equals r.VeiculoId
                        where r.Id == reserva.Id
                        select v.idEmpresa).FirstOrDefault();
            //query = query.FirstOrDefault();
            if (reserva.Estado == ESTADO.Pendente) 
            {
                if (VeiculoReservaEmpresaId == user.EmpresaId && (User.IsInRole("Gestor") || User.IsInRole("Funcionario")))
                {
                    reserva.Estado = ESTADO.Recolher;
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                else 
                {
                    TempData["Error"] = String.Format("Utilizador sem autoridade para realizar operacao");
                }
            }
            else 
            {
                TempData["Error"] = String.Format("Reserva nao se encontra pendente");
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> TratarVeiculoReserva(int? id) 
        {
            var user = GetCurrentUser();
            var reserva = _context.Reserva.Where(c => c.Id == id).FirstOrDefault();

            var VeiculoReservaEmpresaId = (from v in _context.Veiculo
                                           join r in _context.Reserva on v.Id equals r.VeiculoId
                                           where r.Id == reserva.Id
                                           select v.idEmpresa).FirstOrDefault();
            //query = query.FirstOrDefault();
            if (reserva.Estado == ESTADO.Recolher || reserva.Estado == ESTADO.Entregar)
            {
                if (VeiculoReservaEmpresaId == user.EmpresaId && (User.IsInRole("Gestor") || User.IsInRole("Funcionario")))
                {
                    return RedirectToCreateEstado(user.Id, reserva.Id, reserva.Estado);
                }
                else
                {
                    TempData["Error"] = String.Format("Utilizador sem autoridade para realizar operacao");
                }
            }
            else
            {
                TempData["Error"] = String.Format("Reserva nao se encontra em Recolha ou em Entrega");
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RedirectToCreateEstado(string FuncionarioId, int ReservaId, ESTADO EstadoReserva)
        {
            return RedirectToAction("Create", "EstadoVeiculos", new { FuncionarioId = FuncionarioId, ReservaId = ReservaId, EstadoReserva = EstadoReserva });
        }


        // GET: Reservas/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo, "Id", "Id");
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Estado,DataRecolha,DataEntrega,VeiculoId,ClienteId")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Users, "Id", "Id", reserva.ClienteId);
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo, "Id", "Id", reserva.VeiculoId);
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reserva == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Users, "Id", "Id", reserva.ClienteId);
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo, "Id", "Id", reserva.VeiculoId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Estado,DataRecolha,DataEntrega,VeiculoId,ClienteId")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Users, "Id", "Id", reserva.ClienteId);
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo, "Id", "Id", reserva.VeiculoId);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reserva == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .Include(r => r.Cliente)
                .Include(r => r.Veiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reserva == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reserva'  is null.");
            }
            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva != null)
            {
                _context.Reserva.Remove(reserva);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
          return _context.Reserva.Any(e => e.Id == id);
        }
    }
}
