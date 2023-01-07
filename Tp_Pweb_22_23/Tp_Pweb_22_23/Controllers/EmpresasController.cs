using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tp_Pweb_22_23.Data;
using Tp_Pweb_22_23.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tp_Pweb_22_23.Controllers
{
    public class EmpresasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public EmpresasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Empresas
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
              return View(await _context.Empresa.ToListAsync());
        }

        // GET: Empresas/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Empresa == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresa
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }


        // GET: Empresas/Procura
        [HttpGet]
        public async Task<IActionResult> Procura(string? texto)
        {
            if (texto == null)
            {
                var empresas = await _context.Empresa
                    .ToListAsync();
                return View("Index", empresas);
            }
            else
            {
                var empresasProcura = await _context.Empresa
                    .Where(c => c.Nome.ToLower().Contains(texto.ToLower()))
                    .ToListAsync();
                return View("Index", empresasProcura);
            }
        }

        // GET: Empresas/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empresas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Nome,Ativo")] Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(empresa);
                await _context.SaveChangesAsync();

                var empresaGerada = await _context.Empresa.Where(c => c.Nome == empresa.Nome).FirstOrDefaultAsync();

                var GestorEmpresa = new ApplicationUser
                {
                    PrimeiroNome = "Gestor",
                    UltimoNome = empresa.Nome,
                    NIF = 99999,
                    EmpresaId = empresaGerada.Id,
                    UserName = empresa.Nome + "@gestor.com",
                    Email = empresa.Nome + "@gestor.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    Empresa = empresaGerada
                };
                var user = await _userManager.FindByEmailAsync(GestorEmpresa.Email);
                if (user == null)
                {
                    await _userManager.CreateAsync(GestorEmpresa, "Gestor..00");
                    await _userManager.AddToRoleAsync(GestorEmpresa, Roles.Gestor.ToString());
                    TempData["Msg"] = String.Format(
                    "A Empresa '{0}' foi criada com Sucesso. " +
                    "O Gestor pode agora fazer login a partir do email '{1}' e Password 'Gestor..00'. " +
                    "É aconselhavel que altere a Password!",
                    empresa.Nome, empresa.Nome + "@gestor.com");
                }
                else
                    TempData["Msg"] = String.Format(
                    "A Empresa '{0}' foi criada com Sucesso. " +
                    "O Gestor pode agora fazer login a partir do email '{1}' e Password 'Gestor..00'. " +
                    "É aconselhavel que altere a Password!",
                    empresa.Nome, empresa.Nome + "@gestor.com");
                //var user2 = await _userManager.FindByEmailAsync(GestorEmpresa.Email);
                //await _signInManager.SignInAsync(user2, isPersistent: false);
                return RedirectToAction(nameof(Index));
            }
            return View(empresa);
        }

        // GET: Empresas/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Empresa == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresa.FindAsync(id);
            if (empresa == null)
            {
                return NotFound();
            }
            return View(empresa);
        }

        // POST: Empresas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Classificacao,Ativo")] Empresa empresa)
        {
            if (id != empresa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empresa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpresaExists(empresa.Id))
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
            return View(empresa);
        }

        // GET: Empresas/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Empresa == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresa
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        public async Task<bool> CheckVeiculosEmpresa(int id) 
        {
            var veiculos = await _context.Veiculo.Where(c => c.idEmpresa == id).ToListAsync();
            foreach (var veiculo in veiculos) 
            {
                if (veiculo.idEmpresa == id)
                    return true;
            }

            return false;
        }

        public async Task DeleteUsersAsync(int id) 
        {
            var users = await _context.Users.Where(c => c.EmpresaId == id).ToListAsync();
            foreach(var user in users)
                await _userManager.DeleteAsync(user);
        }


        // POST: Empresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Empresa == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Empresa'  is null.");
            }
            var empresa = await _context.Empresa.FindAsync(id);
            if (empresa != null)
            {
                if(await CheckVeiculosEmpresa(id) == true)
                {
                    TempData["Erro"] = String.Format("A Empresa '{0}' possui Veiculos por isso não pode ser apagada",empresa.Nome);
                    return RedirectToAction(nameof(Delete));
                }
                await DeleteUsersAsync(id);
                _context.Empresa.Remove(empresa);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpresaExists(int id)
        {
          return _context.Empresa.Any(e => e.Id == id);
        }
    }
}
