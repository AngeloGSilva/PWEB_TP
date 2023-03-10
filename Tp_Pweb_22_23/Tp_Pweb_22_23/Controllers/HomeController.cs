using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Security.Claims;
using System.Security.Principal;
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


        public async Task<IActionResult> ordenarVeiculosPreco([Bind("Localizacao,DataRecolha,DataEntrega,IdCategoria")] SearchViewModel search, string ordem)
        {

            return View(ordem);
        }


        public IEnumerable<Reserva> getReservasVeiculo(Veiculo veiculo)
        {
            List<Reserva> reservasDoVeiculo = new List<Reserva>();
            List<Reserva> reservas = _context.Reserva.ToList();
            if (reservas.Count == 0)
                return null;
            foreach (var reserva in reservas)
            {
                if (reserva.VeiculoId == veiculo.Id)
                {
                    reservasDoVeiculo.Add(reserva);
                }
            }

            if (reservasDoVeiculo.Count != 0)
            {
                return reservasDoVeiculo;
            }
            return null;
        }


        private bool IsValidDate(DateTime? Recolha, DateTime? Entrega, Reserva reserva)
        {
            if (Recolha > reserva.DataEntrega)
            {
                return true;
            }
            else if (Recolha < reserva.DataRecolha && Entrega < reserva.DataRecolha)
            {
                return true;
            }
            else if (Entrega < reserva.DataEntrega && Recolha < reserva.DataEntrega)
            {
                return false;
            }
            //verificar esta condicap
            else if (Recolha < reserva.DataEntrega)
            {
                return false;
            }
            else if (Recolha == reserva.DataEntrega)
            {
                return false;
            }
            else if (Recolha == reserva.DataRecolha && Entrega == reserva.DataEntrega)
            {
                return false;
            }
            return true;
        }


        public async Task<IActionResult> SearchAsync([Bind("Localizacao,DataRecolha,DataEntrega,IdCategoria")] SearchViewModel search, string? ordem)
        {
                if (search.DataRecolha < search.DataEntrega)
                {
                    var empresa = new Empresa();
                    var searchResultados = new SearchResultadosViewModel();
                    searchResultados.VeiculosDisponiveis = new List<Veiculo>();
                    searchResultados.EmpresasVeiculos = new List<Empresa>();
                    var numeroDeDias = GetNumeroDeDias((DateTime)search.DataRecolha, (DateTime)search.DataEntrega);
                    var veiculosDisponiveis = await _context.Veiculo.Where(c => c.Disponivel == true && c.Localizacao.ToLower() == search.Localizacao.ToLower() && c.idCategoria == search.IdCategoria).ToListAsync();
                    //IEnumerable<Veiculo> veiculosFinal;
                    //IEnumerable<Reserva> reservas = await _context.Reserva.ToListAsync();
                    var flag = false; //para nao entrar no isValidDate mais q uma vez
                    foreach (var veiculo in veiculosDisponiveis)
                    {
                        flag = false;
                        if (getReservasVeiculo(veiculo) != null)
                        {
                            foreach (var reserva in getReservasVeiculo(veiculo))
                            {
                                if (IsValidDate(search.DataRecolha, search.DataEntrega, reserva) && !flag)
                                {
                                    flag = true;
                                    searchResultados.EmpresasVeiculos.Add(await _context.Empresa.Where(e => e.Id == veiculo.idEmpresa).FirstAsync());
                                    searchResultados.VeiculosDisponiveis.Add(veiculo);
                                    searchResultados.TotalDias = numeroDeDias;
                                }
                            }
                        }
                        else
                        {
                            empresa = await _context.Empresa.Where(e => e.Id == veiculo.idEmpresa).FirstAsync();
                            if (!searchResultados.EmpresasVeiculos.Contains(empresa))
                            {
                                searchResultados.EmpresasVeiculos.Add(empresa);
                            }
                            searchResultados.VeiculosDisponiveis.Add(veiculo);
                            searchResultados.TotalDias = numeroDeDias;
                        }
                    }

                    //}
                    searchResultados.DataEntrega = search.DataEntrega;
                    searchResultados.DataRecolha = search.DataRecolha;
                    if (ordem == null) 
                    {
                        return View(searchResultados);
                    }
                    else if (ordem.Equals("asc"))
                    {
                        var list = searchResultados.VeiculosDisponiveis.OrderBy(x => x.Preco).ToList();
                        searchResultados.VeiculosDisponiveis = list;
                        return View(searchResultados);
                    } else if (ordem.Equals("desc")) 
                    {
                        var list = searchResultados.VeiculosDisponiveis.OrderByDescending(x => x.Preco).ToList();
                        searchResultados.VeiculosDisponiveis = list;
                        return View(searchResultados);
                    }
  
                }
                else
                    TempData["Erro"] = String.Format("Datas nao permitidas.");
            
            return View(nameof(Index));
        }


        private ApplicationUser GetCurrentUser()
        {
            var user = _context.Users
                .Where(u => u.UserName == User.Identity.Name)
                .Include(u => u.Empresa)
                .FirstOrDefault();
            return user;
        }

        private int GetNumeroDeDias(DateTime start, DateTime endDate)
        {
            start = start.Date;
            endDate = endDate.Date;

            TimeSpan difference = endDate - start;

            int numberOfDays = difference.Days;
            return (numberOfDays);
        }

        public async Task<IActionResult> FazReservaAsync([Bind("IdVeiculo,DataRecolha,DataEntrega")] FazReservaViewModel reservaSolicitada)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Erro"] = String.Format("Crie uma conta para poder Reservar!");
                return Redirect("/Identity/Account/Register");
            } else if (User.IsInRole("Gestor")|| User.IsInRole("Funcionario")) 
            {
                TempData["Erro"] = String.Format("Contas associadas a empresas não podem fazer reservas!");
                return View(nameof(Index));
            }

            var veiculo = await _context.Veiculo.Include("Categoria").Include("Empresa").Where(c => c.Id == reservaSolicitada.IdVeiculo).FirstAsync();
            var numeroDays = GetNumeroDeDias(reservaSolicitada.DataRecolha, reservaSolicitada.DataEntrega);

            var reserva = new Reserva
            {
                DataRecolha = reservaSolicitada.DataRecolha,
                DataEntrega = reservaSolicitada.DataEntrega,
                VeiculoId = reservaSolicitada.IdVeiculo,
                Total = veiculo.Preco * numeroDays,
                Veiculo = veiculo,
                ClienteId = GetCurrentUser().Id,
                Cliente = GetCurrentUser()
            };
            _context.Add(reserva);
            await _context.SaveChangesAsync();
            //var veiculo = await _context.Veiculo.Include("reservas").Where(c => c.Id == reservaSolicitada.IdVeiculo).FirstAsync();
            return RedirectToAction("Details", "Reservas", new { id = reserva.Id });
        }

        public IActionResult Index()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria.ToList(), "Id", "Nome");
            return View();
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}