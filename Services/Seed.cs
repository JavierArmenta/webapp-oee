using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Enums;
using WebApp.Models;

namespace WebApp.Services
{
    public static class Seed
    {
        public static void SeedDB(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext? context = null)
        {
            SeedRoles(roleManager);
            SeedGodAdmin(userManager);

            if (context != null)
            {
                SeedMenus(context, roleManager);
            }
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Enum.GetNames(typeof(Roles)))
            {
                if (!roleManager.RoleExistsAsync(role).Result)
                {
                    roleManager.CreateAsync(new IdentityRole(role)).Wait();
                }
            }
        }

        private static void SeedGodAdmin(UserManager<ApplicationUser> userManager)
        {
            // 🔹 Leer desde ENV
            var adminEmail = Environment.GetEnvironmentVariable("SEED_ADMIN_EMAIL");
            var adminPassword = Environment.GetEnvironmentVariable("SEED_ADMIN_PASSWORD");
            var adminUserName = Environment.GetEnvironmentVariable("SEED_ADMIN_USERNAME");
            var adminFirstName = Environment.GetEnvironmentVariable("SEED_ADMIN_FIRSTNAME");
            var adminLastName = Environment.GetEnvironmentVariable("SEED_ADMIN_LASTNAME");

            // Validación mínima
            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new Exception("Seed admin variables are not defined in environment");
            }

            // Si el usuario no existe
            if (userManager.FindByEmailAsync(adminEmail).Result == null)
            {
                var adminUserInitial = new ApplicationUser
                {
                    UserName = adminUserName ?? adminEmail,
                    Email = adminEmail,
                    FirstName = adminFirstName ?? "Admin",
                    LastName = adminLastName ?? "System",
                    EmailConfirmed = true
                };

                IdentityResult result =
                    userManager.CreateAsync(adminUserInitial, adminPassword).Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(adminUserInitial, Roles.SuperAdmin.ToString()).Wait();
                    userManager.AddToRoleAsync(adminUserInitial, Roles.Admin.ToString()).Wait();
                    userManager.AddToRoleAsync(adminUserInitial, Roles.User.ToString()).Wait();
                }
            }
        }

        private static void SeedMenus(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            // Si ya hay menús, no hacer nada
            if (context.MenuItems.Any())
            {
                return;
            }

            // Obtener roles
            var adminRole = roleManager.FindByNameAsync(Roles.Admin.ToString()).Result;
            var superAdminRole = roleManager.FindByNameAsync(Roles.SuperAdmin.ToString()).Result;

            if (adminRole == null || superAdminRole == null)
            {
                return;
            }

            // Crear menús padre
            var menuOperadores = new MenuItem { Nombre = "Admon Operadores", Icono = "ph-users", Orden = 1 };
            var menuPlanta = new MenuItem { Nombre = "Planta", Icono = "ph-buildings", Orden = 2 };
            var menuUsuariosWeb = new MenuItem { Nombre = "Usuarios Web", Icono = "ph-user-circle", Orden = 3 };
            var menuLinealytics = new MenuItem { Nombre = "Linealytics", Icono = "ph-chart-line", Orden = 4 };

            context.MenuItems.AddRange(menuOperadores, menuPlanta, menuUsuariosWeb, menuLinealytics);
            context.SaveChanges();

            // Crear submenús de Operadores
            var subMenuOperadores = new MenuItem { Nombre = "Operadores", Url = "~/Operadores", ParentId = menuOperadores.Id, Orden = 1 };
            var subMenuRolesOp = new MenuItem { Nombre = "Departamentos", Url = "~/DepartamentosOperador", ParentId = menuOperadores.Id, Orden = 2 };

            // Crear submenús de Planta
            var subMenuAreas = new MenuItem { Nombre = "Areas", Url = "~/Planta/Areas", ParentId = menuPlanta.Id, Orden = 1 };
            var subMenuLineas = new MenuItem { Nombre = "Lineas", Url = "~/Planta/Lineas", ParentId = menuPlanta.Id, Orden = 2 };
            var subMenuEstaciones = new MenuItem { Nombre = "Estaciones", Url = "~/Planta/Estaciones", ParentId = menuPlanta.Id, Orden = 3 };
            var subMenuMaquinas = new MenuItem { Nombre = "Maquinas", Url = "~/Planta/Maquinas", ParentId = menuPlanta.Id, Orden = 4 };

            // Crear submenús de Usuarios Web
            var subMenuRegistrar = new MenuItem { Nombre = "Registrar usuario", Url = "~/Identity/Account/Register", ParentId = menuUsuariosWeb.Id, Orden = 1 };
            var subMenuModificar = new MenuItem { Nombre = "Modificar usuario", Url = "~/Admin", ParentId = menuUsuariosWeb.Id, Orden = 2 };
            var subMenuRoles = new MenuItem { Nombre = "Administrar Roles", Url = "~/Admin/Roles", ParentId = menuUsuariosWeb.Id, Orden = 3 };
            var subMenuMenus = new MenuItem { Nombre = "Administrar Menús", Url = "~/Admin/Menus", ParentId = menuUsuariosWeb.Id, Orden = 4 };

            // Crear submenús de Linealytics
            var subMenuLinealytics = new MenuItem { Nombre = "Dashboard", Url = "~/Linealytics", ParentId = menuLinealytics.Id, Orden = 1 };

            context.MenuItems.AddRange(
                subMenuOperadores, subMenuRolesOp,
                subMenuAreas, subMenuLineas, subMenuEstaciones, subMenuMaquinas,
                subMenuRegistrar, subMenuModificar, subMenuRoles, subMenuMenus,
                subMenuLinealytics
            );
            context.SaveChanges();

            // Asignar permisos a Admin y SuperAdmin para todos los menús
            var allMenus = context.MenuItems.ToList();
            var roleIds = new[] { adminRole.Id, superAdminRole.Id };

            foreach (var menu in allMenus)
            {
                foreach (var roleId in roleIds)
                {
                    context.MenuRolePermissions.Add(new MenuRolePermission
                    {
                        MenuItemId = menu.Id,
                        RoleId = roleId
                    });
                }
            }

            context.SaveChanges();
        }
    }
}