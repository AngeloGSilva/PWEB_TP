using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tp_Pweb_22_23.Models.ViewModels;
using Tp_Pweb_22_23.Models;
using Microsoft.EntityFrameworkCore;
using Tp_Pweb_22_23.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace Tp_Pweb_22_23.Controllers
{
    public class UserRolesManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserRolesManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
       RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        private ApplicationUser GetCurrentUser()
        {
            var user = _context.Users
                .Where(u => u.UserName == User.Identity.Name)
                .Include(u => u.Empresa)
                .FirstOrDefault();
            return user;
        }

        [HttpPost]
        public ActionResult Talepler(IFormCollection formCollection, string UserId)
        {
            bool chkeco = false, chkbuss = false;
            string chkecoValue = "";
            string chkbussValue = "";
            if (!string.IsNullOrEmpty(formCollection["chkeco"]))
            {
                chkeco = true;
            }

            return View("Index");
        }
        [Authorize(Roles = "Gestor,Admin")]
        public async Task<IActionResult> Index(bool? activo)
        {
            var UtilizadorAtual = await _userManager.GetUserAsync(User);
            List<UserRolesViewModel> userRolesManagerViewModel = new List<UserRolesViewModel>();

            if (User.IsInRole("Admin"))
            {
                var users = new List<ApplicationUser>();
                if (activo == false)
                {
                    users = await _userManager.Users.Include("Empresa").Where(u => u.IsActive == false).ToListAsync();
                }
                else if (activo == true)
                    users = await _userManager.Users.Include("Empresa").Where(u => u.IsActive == true).ToListAsync();
                else
                    users = await _userManager.Users.Include("Empresa").ToListAsync();
                foreach (var user in users)
                {
                        UserRolesViewModel userRolesViewModel = new UserRolesViewModel();
                    if(user.Empresa != null)
                        userRolesViewModel.EmpresaNome = user.Empresa.Nome;
                        
                        userRolesViewModel.Avatar = user.Avatar;
                        userRolesViewModel.UserId = user.Id;
                        userRolesViewModel.UserName = user.UserName;
                        userRolesViewModel.PrimeiroNome = user.PrimeiroNome;
                        userRolesViewModel.UltimoNome = user.UltimoNome;
                        userRolesViewModel.Activo = user.IsActive;
                        userRolesViewModel.Roles = await _userManager.GetRolesAsync(user);

                        userRolesManagerViewModel.Add(userRolesViewModel);
                }
            }
            else
            {
                var users = new List<ApplicationUser>();
                if (activo == false)
                {
                    users = await _context.Users.Include("Empresa").Where(c => c.EmpresaId == UtilizadorAtual.EmpresaId && c.EmpresaId != null && c.IsActive == false).ToListAsync();
                }
                else if (activo == true)
                    users = await _context.Users.Include("Empresa").Where(c => c.EmpresaId == UtilizadorAtual.EmpresaId && c.EmpresaId != null && c.IsActive == true).ToListAsync();
                else
                    users = await _context.Users.Include("Empresa").Where(c => c.EmpresaId == UtilizadorAtual.EmpresaId && c.EmpresaId != null).ToListAsync();

                foreach (var user in users)
                {
                        UserRolesViewModel userRolesViewModel = new UserRolesViewModel();

                        userRolesViewModel.EmpresaNome = user.Empresa.Nome;
                        userRolesViewModel.Avatar = user.Avatar;
                        userRolesViewModel.UserId = user.Id;
                        userRolesViewModel.UserName = user.UserName;
                        userRolesViewModel.PrimeiroNome = user.PrimeiroNome;
                        userRolesViewModel.UltimoNome = user.UltimoNome;
                        userRolesViewModel.Activo = user.IsActive;
                        userRolesViewModel.Roles = await _userManager.GetRolesAsync(user);

                        userRolesManagerViewModel.Add(userRolesViewModel);
                }
            }
            return View(userRolesManagerViewModel);
        }
        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Roles = "Gestor,Admin")]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.Include("Empresa")
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        [Authorize(Roles = "Gestor,Admin")]
        // GET: UserManager/Create
        public IActionResult Create()
        {
            ViewData["Roles"] = new SelectList(new String[] { "Gestor", "Funcionario" });
            return View();
        }

        // POST: UserManager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gestor,Admin")]
        public async Task<IActionResult> Create([Bind("Email,PrimeiroNome,UltimoNome,Password,Activo,Role")] CriarFuncionarioViewModel criarUtilizador)
        {
            ViewData["Roles"] = new SelectList(new String[] { "Gestor", "Funcionario" });
            if (ModelState.IsValid)
            {
                var userAtual = GetCurrentUser();
                var user = new ApplicationUser
                {
                    Email = criarUtilizador.Email,
                    UserName = criarUtilizador.Email,
                    PrimeiroNome = criarUtilizador.PrimeiroNome,
                    UltimoNome = criarUtilizador.UltimoNome,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    EmpresaId = userAtual.EmpresaId,
                    Empresa = userAtual.Empresa,
                    IsActive = criarUtilizador.Activo
                };

                var res = await _userManager.CreateAsync(user, criarUtilizador.Password);
                if (!res.Succeeded)
                {
                    foreach (var err in res.Errors)
                    {
                        if (err.Code.Contains("Password"))
                        {
                            ModelState.AddModelError("Password", err.Description);
                        }
                    }
                    //return View(createUser);
                }
                await _userManager.AddToRoleAsync(user, criarUtilizador.Role);
                TempData["Msg"] = String.Format(
                    "Perfil de Funcionário '{0} {1}' foi criado. " +
                    "O login pode ser realizado com o email '{2}' e Password '{3}'.",
                    user.PrimeiroNome, user.UltimoNome, user.UserName, criarUtilizador.Password);

            }
            return RedirectToAction(nameof(Index));
        }

        // GET: UserManager/Edit/5
        [Authorize(Roles = "Gestor,Admin")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var userAtual = GetCurrentUser();
            if (userAtual.Id != id) {
                var model = new EditFuncionarioViewModel
                {
                    //Id = user.Id,
                    PrimeiroNome = user.PrimeiroNome,
                    UltimoNome = user.UltimoNome,
                    Activo = user.IsActive
                };
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: UserManager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gestor,Admin")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,PrimeiroNome,UltimoNome,Activo")] EditFuncionarioViewModel editUser)
        {
            var user = await _context.Users.FindAsync(id);
            //if (user == null || (editUser != null && id != editUser.Id))
            //{
            //    return NotFound();
            //}

            try
            {
                if (editUser.PrimeiroNome != null) user.PrimeiroNome = editUser.PrimeiroNome;
                if (editUser.UltimoNome != null) user.UltimoNome = editUser.UltimoNome;
                user.IsActive = editUser.Activo;
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                TempData["Erro"] = "Erro ao editar o utilizador: " + e.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }

}
