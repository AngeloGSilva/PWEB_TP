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
        public async Task<IActionResult> Index(bool? activo)
        {
            var UtilizadorAtual = await _userManager.GetUserAsync(User);
            List<UserRolesViewModel> userRolesManagerViewModel = new List<UserRolesViewModel>();

            if (User.IsInRole("Admin"))
            {
                var users = new List<ApplicationUser>();
                if (activo == false)
                {
                    users = await _userManager.Users.Where(u => u.IsActive == false).ToListAsync();
                }
                else if (activo == true)
                    users = await _userManager.Users.Where(u => u.IsActive == true).ToListAsync();
                else
                    users = await _userManager.Users.ToListAsync();
                foreach (var user in users)
                {
                        UserRolesViewModel userRolesViewModel = new UserRolesViewModel();

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
                    users = await _context.Users.Where(c => c.EmpresaId == UtilizadorAtual.EmpresaId && c.EmpresaId != null && c.IsActive == false).ToListAsync();
                }
                else if (activo == true)
                    users = await _context.Users.Where(c => c.EmpresaId == UtilizadorAtual.EmpresaId && c.EmpresaId != null && c.IsActive == true).ToListAsync();
                else
                    users = await _context.Users.Where(c => c.EmpresaId == UtilizadorAtual.EmpresaId && c.EmpresaId != null).ToListAsync();

                foreach (var user in users)
                {
                        UserRolesViewModel userRolesViewModel = new UserRolesViewModel();

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

        //TODO nao esta a dar
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        //[HttpPost]
        //public async Task<IActionResult> Details(string UserId)
        //{
        //    if (UserId == null || _context.Users == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = _context.Users.Where(c => c.Id == UserId);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(user);
        //}


        [Authorize(Roles = "Gestor")]
        // GET: UserManager/Create
        public IActionResult Create()
        {
            //var gestor = _context.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            //if (gestor == null || gestor.EmpresaId == null)
            //{
            //    return NotFound();
            //}
            //var user = new ApplicationUser
            //{
            //    PrimeiroNome = gestor.Empresa.Nome,
            //    UltimoNome = "Funcionario",
            //    UserName = 
            //    EmpresaId = gestor.EmpresaId
            //};

            //result = await _userManager.AddToRolesAsync(user,
            //    model.Where(x => x.Selected).Select(y => y.RoleName));


            ViewData["Roles"] = new SelectList(new String[] { "Gestor", "Funcionario" });
            return View();
        }

        // POST: UserManager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gestor")]
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
                TempData["Info"] = String.Format(
                    "Profile for employee '{0} {1}' was created. " +
                    "He/she can now login with Username '{2}' and Password '{3}'.",
                    user.PrimeiroNome, user.UltimoNome, user.UserName, criarUtilizador.Password);

            }
            return RedirectToAction(nameof(Index));
        }

        // GET: UserManager/Edit/5
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
                TempData["Error"] = "Error while editing User: " + e.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        //// GET: UserManager/Delete/5
        //public async Task<IActionResult> Delete(string? id)
        //{
        //    if (id == null || _context.Users == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await _context.Users
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    if (user.UserName == User.Identity.Name)
        //    {
        //        TempData["Error"] = "You cannot delete your own profile!";
        //        return RedirectToAction("Index");
        //    }

        //    return View(user);
        //}
    }

}
