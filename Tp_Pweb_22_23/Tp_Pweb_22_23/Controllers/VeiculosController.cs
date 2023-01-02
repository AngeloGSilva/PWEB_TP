using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tp_Pweb_22_23.Data;
using Tp_Pweb_22_23.Models;
using Tp_Pweb_22_23.Models.ViewModels;

namespace Tp_Pweb_22_23.Controllers
{
    public class VeiculosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VeiculosController(ApplicationDbContext context)
        {
            _context = context;
        }

        //verifica se a extensão é .png,.jpg,.jpeg
        public bool isValidFileType(string filename)
        {
            List<string> fileExtensions = new List<string>() { "png", "jpg", "jpeg" };
            List<string> filenameSeparated = filename.Split('.').Reverse().ToList<string>();

            foreach (var extension in fileExtensions)
                if (extension.Equals(filenameSeparated[0]))
                    return true;

            return false;
        }

        public async Task<IActionResult> AllVeiculos() 
        {
            var veiculos = new AllVeiculosViewModel();
            veiculos.ListaDeVeiculos = await _context.Veiculo.Where(c => c.Disponivel == true).ToListAsync();
            veiculos.NumResultados = veiculos.ListaDeVeiculos.Count;
            return View(veiculos);
        }


        // GET: Veiculos
        public async Task<IActionResult> Index()
        {
              return View(await _context.Veiculo.ToListAsync());
        }

        // GET: Veiculos/Details/5
        public async Task<IActionResult> Details(int? id)
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

            return View(veiculo);
        }

        // GET: Veiculos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Veiculos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Foto,Marca,Disponivel,Danos,Condicao,Localizacao,Preco,idEmpresa,idCategoria")] Veiculo veiculo, IFormFile FotoVeiculo)
        {
            if (ModelState.IsValid)
            {
                if (FotoVeiculo.Length <= (200 * 1024) && isValidFileType(FotoVeiculo.FileName))
                {
                    using (var dataStream = new MemoryStream())
                    {
                        await FotoVeiculo.CopyToAsync(dataStream);
                        veiculo.Foto = dataStream.ToArray();
                    }
                    _context.Add(veiculo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = String.Format(
                    "Imagem demasiado Grande.");
                }
            }
            return View(veiculo);
        }

        // GET: Veiculos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Veiculo == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculo.FindAsync(id);
            if (veiculo == null)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Foto,Marca,Disponivel,Danos,Condicao,Localizacao,Preco,idEmpresa,idCategoria")] Veiculo veiculo)
        {
            if (id != veiculo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(veiculo);
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

            return View(veiculo);
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
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VeiculoExists(int id)
        {
          return _context.Veiculo.Any(e => e.Id == id);
        }
    }
}
