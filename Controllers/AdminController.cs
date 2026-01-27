using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrator")]
    public class AdminController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private IPasswordHasher<ApplicationUser> passwordHasher;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> usrMgr, IPasswordHasher<ApplicationUser> passwordHash, RoleManager<IdentityRole> roleManager)
        {
            userManager = usrMgr;
            passwordHasher = passwordHash;
            _roleManager = roleManager;
        }

        public IActionResult Index(bool showInactive = false)
        {
            var users = showInactive
                ? userManager.Users
                : userManager.Users.Where(u => u.IsActive);

            ViewBag.ShowInactive = showInactive;
            return View(users);
        }

        public async Task<IActionResult> Update(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(ApplicationUser model, string? password)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(model.Id);
            if (user != null)
            {
                // Actualizar campos básicos
                user.UserName = model.UserName;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;

                // Validar email
                if (string.IsNullOrEmpty(user.Email))
                {
                    ModelState.AddModelError("", "El email no puede estar vacío");
                }

                // Solo actualizar contraseña si se proporciona una nueva
                if (!string.IsNullOrEmpty(password))
                {
                    user.PasswordHash = passwordHasher.HashPassword(user, password);
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        Errors(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Usuario no encontrado");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Deactivate(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = false;
            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Opcional: cerrar sesiones activas del usuario
                await userManager.UpdateSecurityStampAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Activa un usuario
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Activate(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = true;
            await userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", userManager.Users);
        }

        void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        /// added 
        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await userManager.GetRolesAsync(user));
        }

        public async Task<IActionResult> Manage(string userId)
        {
            ViewBag.userId = userId;
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            ViewBag.UserName = user.UserName;
            var model = new List<ManageUserRolesViewModel>();
            foreach (var role in _roleManager.Roles.ToList())
            {
                var userRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View();
            }
            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        // ==================== CRUD de Roles ====================

        /// <summary>
        /// Lista todos los roles del sistema
        /// </summary>
        public IActionResult Roles()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        /// <summary>
        /// Muestra formulario para crear un nuevo rol
        /// </summary>
        public IActionResult CreateRole()
        {
            return View();
        }

        /// <summary>
        /// Crea un nuevo rol
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError("", "El nombre del rol es requerido");
                return View();
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                ModelState.AddModelError("", "El rol ya existe");
                return View();
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                TempData["Success"] = $"Rol '{roleName}' creado exitosamente";
                return RedirectToAction(nameof(Roles));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        /// <summary>
        /// Muestra formulario para editar un rol
        /// </summary>
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        /// <summary>
        /// Actualiza el nombre de un rol
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> EditRole(string id, string roleName)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError("", "El nombre del rol es requerido");
                return View(role);
            }

            var existingRole = await _roleManager.FindByNameAsync(roleName);
            if (existingRole != null && existingRole.Id != id)
            {
                ModelState.AddModelError("", "Ya existe un rol con ese nombre");
                return View(role);
            }

            role.Name = roleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                TempData["Success"] = $"Rol actualizado exitosamente";
                return RedirectToAction(nameof(Roles));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(role);
        }

        /// <summary>
        /// Elimina un rol
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            // Verificar si hay usuarios con este rol
            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);
            if (usersInRole.Any())
            {
                TempData["Error"] = $"No se puede eliminar el rol '{role.Name}' porque tiene {usersInRole.Count} usuario(s) asignado(s)";
                return RedirectToAction(nameof(Roles));
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["Success"] = $"Rol '{role.Name}' eliminado exitosamente";
            }
            else
            {
                TempData["Error"] = "Error al eliminar el rol";
            }

            return RedirectToAction(nameof(Roles));
        }
    }
}
