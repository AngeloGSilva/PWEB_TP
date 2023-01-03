using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Tp_Pweb_22_23.Data;
using Tp_Pweb_22_23.Models;
using Tp_Pweb_22_23.Models.ViewModels;

namespace Tp_Pweb_22_23.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IEnumerable<Reserva> getReservasVeiculo(Veiculo veiculo) 
        {
            List<Reserva> reservas = new List<Reserva>();
            List<Reserva> reservasDoVeiculo = _context.Reserva.ToList();

            foreach (var reserva in reservasDoVeiculo) 
            {
                if (reserva.VeiculoId == veiculo.Id) 
                {
                    reservas.Add(reserva);
                }
            }
            return reservas;
        }

        public async Task<IActionResult> SearchAsync( [Bind("Localizacao,DataRecolha,DataEntrega,Categoria")] SearchViewModel search) 
        {
            List<Veiculo> veiculosDisponiveis = new List<Veiculo>();

            if (search.DataRecolha == null && search.DataEntrega == null)
            {
                veiculosDisponiveis = await _context.Veiculo.Where(c => c.Disponivel == true && c.Localizacao == search.Localizacao).ToListAsync();
                return View(veiculosDisponiveis);
            }
            else 
            {
                veiculosDisponiveis = await _context.Veiculo.Where(c => c.Disponivel == true && c.Localizacao == search.Localizacao).ToListAsync();
                IEnumerable<Veiculo> veiculosFinal;
                //IEnumerable<Reserva> reservas = await _context.Reserva.ToListAsync();
                var flag = false;
                foreach (var veiculo in veiculosDisponiveis) 
                {
                    foreach (var reserva in getReservasVeiculo(veiculo)) 
                    {
                        if (search.DataRecolha <= reserva.DataRecolha || search.DataEntrega >= reserva.DataEntrega) 
                        {
                            veiculosDisponiveis.Add(veiculo);
                        }
                    }
                }

            }
            return View(veiculosDisponiveis);
        }

        public IActionResult Index()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria.ToList(), "Id", "Nome");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}