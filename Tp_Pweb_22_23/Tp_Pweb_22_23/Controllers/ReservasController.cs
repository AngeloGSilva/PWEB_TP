using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tp_Pweb_22_23.Data;
using Tp_Pweb_22_23.Models;
using Tp_Pweb_22_23.Models.ViewModels;

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


        public async Task<IActionResult> Classificar(int idEmpresa, int avaliacao) 
        {
            var empresa = await _context.Empresa.Include("Veiculos").Include("Funcionarios").Where(e=> e.Id == idEmpresa).FirstAsync();
            empresa.NrClassificacoes = empresa.NrClassificacoes + 1;
            empresa.SomaClassificacoes = empresa.SomaClassificacoes + avaliacao;
            empresa.Classificacao = empresa.SomaClassificacoes / empresa.NrClassificacoes;
            _context.Update(empresa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ClassificarEmpresa(int? id) 
        {
            var classificaViewModel = new ClassificaEmpresaViewModel();
            var cliente = GetCurrentUser();
            if (cliente == null) return NotFound();
            var reserva = await _context.Reserva.Include("Veiculo").Include("estadoVeiculos").Include("Cliente").Where(c=> c.Id == id).FirstAsync();
            var veiculo = await _context.Veiculo.Include("Empresa").Include("Categoria").Where(v => v.Id == reserva.VeiculoId).FirstAsync();
            var empresa = await _context.Empresa.Include("Veiculos").Include("Funcionarios").Where(e => e.Id == veiculo.idEmpresa).FirstAsync();
            if (reserva == null) return NotFound();
            if (reserva.ClienteId == cliente.Id && reserva.Estado == ESTADO.Classificar)
            {
                classificaViewModel.empresa = empresa;
                classificaViewModel.reserva = reserva;
                classificaViewModel.cliente= cliente;
                classificaViewModel.veiculo = veiculo;
            }
            return View(classificaViewModel);
        }


        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            ViewData["ClienteId"] = new SelectList(_context.Users.ToList(), "Id", "Email");
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo.ToList(), "Id", "Marca");
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo.ToList(), "Id", "Modelo");


            var user = GetCurrentUser();

            if (User.IsInRole("Gestor") || User.IsInRole("Funcionario"))
            {
                var reservasComVeiculosDaMesmaEmpresa = from r in _context.Reserva.Include("Veiculo")
                                                        join v in _context.Veiculo.Include("Empresa") on r.VeiculoId equals v.Id
                                                        where v.idEmpresa == user.EmpresaId
                                                        select r;

                return View(reservasComVeiculosDaMesmaEmpresa);
            }
            if (User.IsInRole("Cliente"))
            {
                var reservasCliente = await _context.Reserva.Include("Veiculo").Include("Veiculo.Empresa").Where(r => r.ClienteId == user.Id).ToListAsync();
                //var reservasCliente = await _context.Reserva.Include("Veiculo").Where(r => r.Estado == ESTADO.Concluida || _context.Veiculo.Include("Empresa").Any(v => v.Id == r.VeiculoId && _context.Empresa.Include("Veiculos").Any(e => e.Id == v.idEmpresa && e.Ativo == true))).ToListAsync();
                //_context.Veiculo.Where(v => _context.Empresa.Any(e => e.Id == v.idEmpresa && e.Ativo == true) && v.Disponivel == true);
                return View(reservasCliente);
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
            var veiculoReserva = await _context.Veiculo.Include("Empresa").Where(v => v.Id == reserva.VeiculoId).FirstAsync();
            reserva.Veiculo = veiculoReserva;

            return View(reserva);
        }


        public async Task<IActionResult> ConfirmReserva(int? id)
        {
            var user = GetCurrentUser();
            var reserva = _context.Reserva.Include("Cliente").Include("Veiculo").Where(c => c.Id == id).FirstOrDefault();
            var veiculo2 = reserva.Veiculo;
            var VeiculoReservaEmpresaId = (from v in _context.Veiculo
                                           join r in _context.Reserva on v.Id equals r.VeiculoId
                                           where r.Id == reserva.Id
                                           select v.idEmpresa).FirstOrDefault();
            //query = query.FirstOrDefault();
            if (reserva.Estado == ESTADO.Pendente)
            {
                if (VeiculoReservaEmpresaId == user.EmpresaId && (User.IsInRole("Gestor") || User.IsInRole("Funcionario")))
                {
                    //reserva.Veiculo = await _context.Veiculo.Where(v => v.Id == reserva.VeiculoId).FirstAsync();
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

        public async Task<IActionResult> RecusarReserva(int? id)
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
                    reserva.Estado = ESTADO.Cancelada;
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
                reserva.Veiculo = await _context.Veiculo.Where(v => v.Id == reserva.VeiculoId).FirstAsync();
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




        //DAshBoard

        public IActionResult VendasMensaisBar()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DadosVendasMensaisBar()
        {
            List<object>? VendasMes = new List<object>();

            DataTable dt = new DataTable();
            dt.Columns.Add("Vendas", System.Type.GetType("System.String"));
            dt.Columns.Add("Quantidade", System.Type.GetType("System.Int32"));

            var veiculos = _context.Veiculo
                .GroupBy(x => new { x.Id, x.Marca })
                            .Select(x => new
                            {
                                x.Key.Id,
                                x.Key.Marca,
                                vendasNoMes = _context.Reserva
                                                    .Where(p => p.VeiculoId == x.Key.Id)}).OrderBy(x => x.Marca).ToList();

            //Percorrendo e extraindo os dados de venda de cada curso na BD para os inserir nas DataRow
            foreach (var veiculo in veiculos)
            {
                DataRow dr = dt.NewRow();
                dr["Vendas"] = veiculo.Marca;
                dr["Quantidade"] = veiculo.vendasNoMes;
                dt.Rows.Add(dr);
            }

            //Percorrendo e extraindo cada DataColumn para List<Object>
            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                VendasMes.Add(x);
            }

            //Dados retornados no formato JSON
            return Json(VendasMes);
        }
    }
}
