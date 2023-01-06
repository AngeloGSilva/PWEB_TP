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
            //ViewData["Id"] = new SelectList(_context.Categoria.ToList(), "Id", "Nome");
            ViewData["FuncionarioId"] = new SelectList(_context.Users.ToList(), "Id", "Email");
            //ViewData["VeiculoId"] = new SelectList(_context.Users.ToList(), "Id", "Marca");
            var funcionario = GetCurrentUser();
            var estados = await _context.EstadoVeiculo.Include("Reserva").ToListAsync();
            var estadosVeiculosEmpresa = new List<EstadoVeiculo>();
            foreach (var estado in estados) 
            {
                var veiculo = await _context.Veiculo.Include("Empresa").Include("Categoria").Where(v => v.Id == estado.Reserva.VeiculoId).FirstAsync();
                //var idEmpresa = estado.Reserva.Veiculo.idEmpresa;
                var idEmpresa = _context.Reserva.Where(r => r.Id == estado.ReservaId && r.VeiculoId != null)
                        .Select(r => r.Veiculo.idEmpresa)
                        .FirstOrDefault();
                estado.Reserva.Veiculo = veiculo;

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
            ViewData["FuncionarioId"] = new SelectList(_context.Users.ToList(), "Id", "Email");
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
            string coursePath;

            if(estadoVeiculo.ESTADO == ESTADO.Recolher && estadoVeiculo.Danos) 
            {
                coursePath = Path.Combine(Directory.GetCurrentDirectory(), ("wwwroot/img/Danos/" + estadoVeiculo.ReservaId.ToString() + "/Recolher"));
                if (!Directory.Exists(coursePath))
                    Directory.CreateDirectory(coursePath);
                //LINK SYNTAX
                var files = from file in
                                Directory.EnumerateFiles(coursePath)
                            select string.Format(
                                "/img/Danos//{0}/Recolher/{1}",
                                estadoVeiculo.ReservaId,
                                Path.GetFileName(file));

                ViewData["Ficheiros"] = files; //lista de strings para a vista
            }
                
            if (estadoVeiculo.ESTADO == ESTADO.Entregar && estadoVeiculo.Danos) 
            {
                coursePath = Path.Combine(Directory.GetCurrentDirectory(), ("wwwroot/img/Danos/" + estadoVeiculo.ReservaId.ToString() + "/Entregar"));
                if (!Directory.Exists(coursePath))
                    Directory.CreateDirectory(coursePath);
                //LINK SYNTAX
                var files = from file in
                                Directory.EnumerateFiles(coursePath)
                            select string.Format(
                                "/img/cursos//{0}/Entregar/{1}",
                                estadoVeiculo.ReservaId,
                                Path.GetFileName(file));

                ViewData["Ficheiros"] = files; //lista de strings para a vista
            }
            return View(estadoVeiculo);
        }

        // GET: EstadoVeiculos/Create
        public IActionResult Create(string FuncionarioId, int ReservaId, ESTADO EstadoReserva)
        {
            var reserva = _context.Reserva.Include("Cliente").Include("Veiculo").Where(c => c.Id == ReservaId).FirstOrDefault();
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
        public async Task<IActionResult> Create([Bind("NumeroKm,Danos,Observacoes,FuncionarioId,ReservaId,ESTADO")] EstadoVeiculo estadoVeiculo, [FromForm] List<IFormFile> ficheiros)
        {
            var reserva = _context.Reserva.Include("Veiculo").Include("Cliente").Include("estadoVeiculos").Where(c=> c.Id == estadoVeiculo.ReservaId).FirstOrDefault();
            var funcionario = await _userManager.FindByIdAsync(estadoVeiculo.FuncionarioId);
            estadoVeiculo.Reserva = reserva;
            estadoVeiculo.Funcionario = funcionario;
            if (ModelState.IsValid)
            {
                if (reserva.Estado == ESTADO.Entregar)
                {
                    if (estadoVeiculo.Danos) 
                    {
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Danos/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        // Dir relativo aos ficheiros do curso
                        path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Danos/" + reserva.Id.ToString() + "/Entregar");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);



                        foreach (var formFile in ficheiros)
                        {
                            if (formFile.Length > 0)
                            {
                                var filePath = Path.Combine(path, Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName));
                                while (System.IO.File.Exists(filePath))
                                {
                                    filePath = Path.Combine(path, Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName));
                                }
                                using (var stream = System.IO.File.Create(filePath))
                                {
                                    await formFile.CopyToAsync(stream);
                                }
                            }
                        }



                    }

                    reserva.Estado = ESTADO.Classificar;
                } else if (reserva.Estado == ESTADO.Recolher) 
                {
                    if (estadoVeiculo.Danos)
                    {
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Danos/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        // Dir relativo aos ficheiros do curso
                        path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Danos/" + reserva.Id.ToString() + "/Recolher");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        foreach (var formFile in ficheiros)
                        {
                            if (formFile.Length > 0)
                            {
                                var filePath = Path.Combine(path, Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName));
                                while (System.IO.File.Exists(filePath))
                                {
                                    filePath = Path.Combine(path, Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName));
                                }
                                using (var stream = System.IO.File.Create(filePath))
                                {
                                    await formFile.CopyToAsync(stream);
                                }
                            }
                        }




                    }

                    reserva.Estado = ESTADO.Entregar;
                }
                _context.Add(estadoVeiculo);
                reserva.estadoVeiculos.Add(estadoVeiculo);
                _context.Update(reserva);
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
