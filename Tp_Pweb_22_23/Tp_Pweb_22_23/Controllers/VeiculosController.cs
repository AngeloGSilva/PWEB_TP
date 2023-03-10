using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Drawing;
using Tp_Pweb_22_23.Data;
using Tp_Pweb_22_23.Models;
using Tp_Pweb_22_23.Models.ViewModels;

namespace Tp_Pweb_22_23.Controllers
{
    public class VeiculosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VeiculosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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

        //verifica se a extensão é .png,.jpg,.jpeg
        public bool isValidFileType(string filename)
        {
            List<string> fileExtensions = new List<string>() { "PNG", "png", "jpg", "jpeg","JPG" };
            List<string> filenameSeparated = filename.Split('.').Reverse().ToList<string>();

            foreach (var extension in fileExtensions)
                if (extension.Equals(filenameSeparated[0]))
                    return true;

            return false;
        }

        public async Task<IActionResult> AllVeiculos(string? ordem) 
        {
            var veiculos = new AllVeiculosViewModel();
            ViewData["EmpresaId"] = new SelectList(_context.Empresa.ToList(), "Id", "Nome");
            //veiculos.ListaDeVeiculos = await _context.Veiculo.Where(c => c.Disponivel == true).ToListAsync();
            veiculos.ListaDeVeiculos = await _context.Veiculo.Include("Categoria").Where(v => _context.Empresa.Any(e => e.Id == v.idEmpresa && e.Ativo == true) && v.Disponivel == true).ToListAsync();
            veiculos.NumResultados = veiculos.ListaDeVeiculos.Count;
            if (ordem == null)
            {
                return View(veiculos);
            }
            else if (ordem.Equals("asc"))
            {
                var list = veiculos.ListaDeVeiculos.OrderBy(x => x.Preco).ToList();
                veiculos.ListaDeVeiculos = list;
                return View(veiculos);
            }
            else if (ordem.Equals("desc"))
            {
                var list = veiculos.ListaDeVeiculos.OrderByDescending(x => x.Preco).ToList();
                veiculos.ListaDeVeiculos = list;
                return View(veiculos);
            }
            return View(nameof(AllVeiculos));
        }


        // GET: Empresas/Procura
        [HttpGet]
        public async Task<IActionResult> Procura(string? texto)
        {
            if (texto == null)
            {
                var veiculos = await _context.Veiculo
                    .ToListAsync();
                return View("Index", veiculos);
            }
            else
            {
                var veiculosProcura = await _context.Veiculo
                    .Where(c => c.Marca.ToLower().Contains(texto.ToLower()))
                    .ToListAsync();
                return View("Index", veiculosProcura);
            }
        }


        // GET: Veiculos
        public async Task<IActionResult> Index(bool? activo)
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria.ToList(), "Id", "Nome");
            ViewData["EmpresaId"] = new SelectList(_context.Empresa.ToList(), "Id", "Nome");
            var funcionario = GetCurrentUser();
            var veiculos = new List<Veiculo>();
            if (activo == false) {
                veiculos = await _context.Veiculo.Where(c => c.idEmpresa == funcionario.EmpresaId && c.Disponivel == false).ToListAsync();
            }else if(activo == true)
                veiculos = await _context.Veiculo.Where(c => c.idEmpresa == funcionario.EmpresaId && c.Disponivel == true).ToListAsync();
            else
                veiculos = await _context.Veiculo.Where(c => c.idEmpresa == funcionario.EmpresaId).ToListAsync();
            return View(veiculos);
        }

        // GET: Veiculos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria.ToList(), "Id", "Nome");
            ViewData["EmpresaId"] = new SelectList(_context.Empresa.ToList(), "Id", "Nome");
            if (id == null || _context.Veiculo == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculo == null)
            {
                return NotFound();
            }

            return View(veiculo);
        }

        [Authorize(Roles = "Funcionario,Gestor")]
        // GET: Veiculos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria.ToList(), "Id", "Nome");
            //ViewData["EmpresaId"] = new SelectList(_context.Empresa.Include(c => c.Funcionarios).Include(u => u.Funcionarios).ToList());
            return View();
        }

        // POST: Veiculos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Create([Bind("Id,Foto,Marca,Disponivel,Modelo,Localizacao,Preco,idCategoria")] Veiculo veiculo, IFormFile FotoVeiculo)
        {
            var funcionario = GetCurrentUser();

            if (FotoVeiculo == null)
            {
                TempData["Erro"] = String.Format("Precisa de Colocar foto");
                return RedirectToAction(nameof(Create));
            }
            //var user = _userManage.
            ViewData["CategoriaId"] = new SelectList(_context.Categoria.ToList(), "Id", "Nome");
            if (ModelState.IsValid)
            {
                if (FotoVeiculo.Length <= (400 * 1024) && isValidFileType(FotoVeiculo.FileName))
                {
                    using (var dataStream = new MemoryStream())
                    {
                        await FotoVeiculo.CopyToAsync(dataStream);
                        veiculo.Foto = dataStream.ToArray();
                    }
                    veiculo.idEmpresa = funcionario.EmpresaId;
                    veiculo.Empresa = funcionario.Empresa;
                    var categoria = await _context.Categoria.Where(c => c.Id == veiculo.idCategoria).FirstAsync();
                    veiculo.Categoria = categoria;
                    //TempData["Msg"] = String.Format("Categoria a null ");
                    _context.Add(veiculo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Erro"] = String.Format("Imagem demasiado Grande.");
                    return View(veiculo);
                }
            }
            return View(veiculo);
        }

        // GET: Veiculos/Edit/5
        [Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria.ToList(), "Id", "Nome");
            if (id == null || _context.Veiculo == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculo.FindAsync(id);
            if (veiculo == null)
            {
                return NotFound();
            }

            if (veiculo.idEmpresa != GetCurrentUser().EmpresaId)
            {
                return NotFound();
            }
            return View(veiculo);
        }

        // POST: Veiculos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Marca,Disponivel,Modelo,Localizacao,Preco,idCategoria")] Veiculo veiculo)
        {
            if (id != veiculo.Id)
            {
                return NotFound();
            }

            var veiculoAnterior = await _context.Veiculo.Where(c => c.Id == veiculo.Id).FirstAsync();

            if (veiculoAnterior != veiculo) 
            {
                if(await CheckReservasVeiculoPendentes(veiculo.Id)) 
                {
                    TempData["Erro"] = String.Format("O Veiculo '{0}' '{1}' não pode ser editado enquanto estiver incluido em reservas por concluir.", veiculo.Marca, veiculo.Modelo);
                    //veiculoAnterior.Disponivel = true;
                    //RedirectToAction(nameof(Edit));
                }
                else {
                    veiculoAnterior.Preco = veiculo.Preco;
                    veiculoAnterior.Disponivel = veiculo.Disponivel;
                    veiculoAnterior.Localizacao = veiculo.Localizacao;
                    veiculoAnterior.Marca = veiculo.Marca;
                    veiculoAnterior.Modelo = veiculo.Modelo;
                }

            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(veiculoAnterior);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeiculoExists(veiculo.Id))
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
            return View(veiculo);
        }

        // GET: Veiculos/Delete/5
        [Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Veiculo == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculo == null)
            {
                return NotFound();
            }
            if (veiculo.idEmpresa != GetCurrentUser().EmpresaId) 
            {
                return NotFound();
            }

            return View(veiculo);
        }


        public async Task<bool> CheckReservasVeiculoPendentes(int id)
        {
            var reservas = await _context.Reserva.Where(r => r.VeiculoId == id).ToListAsync();
            foreach (var reserva in reservas)
            {
                if (reserva.Estado != ESTADO.Concluida)
                    return true;
            }
            return false;
        }


        public async Task<bool> CheckReservasVeiculo(int id) 
        {
            var reservas = await _context.Reserva.Where(r => r.VeiculoId == id).ToListAsync();
            foreach (var reserva in reservas)
            {
                if (reserva.VeiculoId == id)
                    return true;
            }
            return false;
        }

        // POST: Veiculos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Veiculo == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Veiculo'  is null.");
            }
            var veiculo = await _context.Veiculo.FindAsync(id);
            if (veiculo != null)
            {
                _context.Veiculo.Remove(veiculo);
            }

            if(await CheckReservasVeiculo(id) == true)
                {
                    TempData["Erro"] = String.Format("O Veiculo '{0}' '{1}' não pode ser apagado enquanto estiver incluido em reservas por concluir.", veiculo.Marca, veiculo.Modelo);
                    return RedirectToAction(nameof(Delete));
                }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VeiculoExists(int id)
        {
          return _context.Veiculo.Any(e => e.Id == id);
        }
    }
}
