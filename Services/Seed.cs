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
                SeedCatalogosMenu(context, roleManager);
                SeedLinealyticsSubmenus(context, roleManager);
                SeedEstadoLineasMenu(context, roleManager);
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
            var subMenuLinealytics = new MenuItem { Nombre = "General", Url = "~/Linealytics", ParentId = menuLinealytics.Id, Orden = 1 };

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

        /// <summary>
        /// Agrega el menú "Catálogos" con sus submenús si aún no existe.
        /// Idempotente: se puede llamar en cada arranque sin duplicar datos.
        /// Visible para todos los roles (SuperAdmin, Admin, User).
        /// </summary>
        private static void SeedCatalogosMenu(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            if (context.MenuItems.Any(m => m.Nombre == "Catálogos" && m.ParentId == null))
                return;

            var superAdminRole = roleManager.FindByNameAsync(Roles.SuperAdmin.ToString()).Result;
            var adminRole = roleManager.FindByNameAsync(Roles.Admin.ToString()).Result;
            var userRole = roleManager.FindByNameAsync(Roles.User.ToString()).Result;

            if (superAdminRole == null || adminRole == null || userRole == null)
                return;

            var menuCatalogos = new MenuItem { Nombre = "Catálogos", Icono = "ph-books", Orden = 5 };
            context.MenuItems.Add(menuCatalogos);
            context.SaveChanges();

            var subMenuProductos  = new MenuItem { Nombre = "Productos",  Url = "~/Linealytics/Productos",  ParentId = menuCatalogos.Id, Orden = 1 };
            var subMenuTurnos     = new MenuItem { Nombre = "Turnos",     Url = "~/Linealytics/Turnos",     ParentId = menuCatalogos.Id, Orden = 2 };
            var subMenuBotones    = new MenuItem { Nombre = "Botones",    Url = "~/Linealytics/Botones",    ParentId = menuCatalogos.Id, Orden = 3 };
            var subMenuBotoneras  = new MenuItem { Nombre = "Botoneras",  Url = "~/Linealytics/Botoneras",  ParentId = menuCatalogos.Id, Orden = 4 };

            context.MenuItems.AddRange(subMenuProductos, subMenuTurnos, subMenuBotones, subMenuBotoneras);
            context.SaveChanges();

            var allMenuIds = new[] { menuCatalogos.Id, subMenuProductos.Id, subMenuTurnos.Id, subMenuBotones.Id, subMenuBotoneras.Id };
            var roleIds    = new[] { superAdminRole.Id, adminRole.Id, userRole.Id };

            foreach (var menuId in allMenuIds)
            {
                foreach (var roleId in roleIds)
                {
                    var exists = context.MenuRolePermissions.Any(p => p.MenuItemId == menuId && p.RoleId == roleId);
                    if (!exists)
                    {
                        context.MenuRolePermissions.Add(new MenuRolePermission { MenuItemId = menuId, RoleId = roleId });
                    }
                }
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Agrega submenús de Fallas, Paros y Contadores bajo el menú Linealytics.
        /// Idempotente.
        /// </summary>
        private static void SeedLinealyticsSubmenus(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            var menuLinealytics = context.MenuItems.FirstOrDefault(m => m.Nombre == "Linealytics" && m.ParentId == null);
            if (menuLinealytics == null)
                return;

            // Renombrar "Dashboard" → "General" si aún existe con el nombre antiguo
            var subDashboard = context.MenuItems.FirstOrDefault(m => m.Nombre == "Dashboard" && m.ParentId == menuLinealytics.Id);
            if (subDashboard != null)
            {
                subDashboard.Nombre = "General";
                context.SaveChanges();
            }

            var nuevos = new[]
            {
                new { Nombre = "Paros",      Url = "~/Linealytics/ParosLinea",       Orden = 2 },
                new { Nombre = "Contadores", Url = "~/Linealytics/Contadores",       Orden = 3 },
                new { Nombre = "Fallas",     Url = "~/Fallas/RegistrosFallas",       Orden = 4 },
            };

            var superAdminRole = roleManager.FindByNameAsync(Roles.SuperAdmin.ToString()).Result;
            var adminRole      = roleManager.FindByNameAsync(Roles.Admin.ToString()).Result;
            var userRole       = roleManager.FindByNameAsync(Roles.User.ToString()).Result;

            if (superAdminRole == null || adminRole == null || userRole == null)
                return;

            var roleIds = new[] { superAdminRole.Id, adminRole.Id, userRole.Id };

            foreach (var def in nuevos)
            {
                var existe = context.MenuItems.Any(m => m.Nombre == def.Nombre && m.ParentId == menuLinealytics.Id);
                if (existe)
                    continue;

                var item = new MenuItem
                {
                    Nombre   = def.Nombre,
                    Url      = def.Url,
                    ParentId = menuLinealytics.Id,
                    Orden    = def.Orden,
                    Activo   = true
                };
                context.MenuItems.Add(item);
                context.SaveChanges();

                foreach (var roleId in roleIds)
                {
                    if (!context.MenuRolePermissions.Any(p => p.MenuItemId == item.Id && p.RoleId == roleId))
                    {
                        context.MenuRolePermissions.Add(new MenuRolePermission { MenuItemId = item.Id, RoleId = roleId });
                    }
                }

                // Asegurar que el padre también tenga permisos para estos roles
                foreach (var roleId in roleIds)
                {
                    if (!context.MenuRolePermissions.Any(p => p.MenuItemId == menuLinealytics.Id && p.RoleId == roleId))
                    {
                        context.MenuRolePermissions.Add(new MenuRolePermission { MenuItemId = menuLinealytics.Id, RoleId = roleId });
                    }
                }
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Agrega "Dashboard de Líneas" bajo el menú Linealytics. Idempotente.
        /// </summary>
        private static void SeedEstadoLineasMenu(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            var menuLinealytics = context.MenuItems.FirstOrDefault(m => m.Nombre == "Linealytics" && m.ParentId == null);
            if (menuLinealytics == null) return;

            if (context.MenuItems.Any(m => m.Nombre == "Dashboard de L\u00edneas" && m.ParentId == menuLinealytics.Id))
                return;

            var superAdminRole = roleManager.FindByNameAsync(Roles.SuperAdmin.ToString()).Result;
            var adminRole      = roleManager.FindByNameAsync(Roles.Admin.ToString()).Result;
            var userRole       = roleManager.FindByNameAsync(Roles.User.ToString()).Result;
            if (superAdminRole == null || adminRole == null || userRole == null) return;

            var item = new MenuItem
            {
                Nombre   = "Dashboard de L\u00edneas",
                Url      = "~/Linealytics/EstadoLineas",
                ParentId = menuLinealytics.Id,
                Orden    = 0,
                Activo   = true
            };
            context.MenuItems.Add(item);
            context.SaveChanges();

            var roleIds = new[] { superAdminRole.Id, adminRole.Id, userRole.Id };
            foreach (var roleId in roleIds)
            {
                if (!context.MenuRolePermissions.Any(p => p.MenuItemId == item.Id && p.RoleId == roleId))
                    context.MenuRolePermissions.Add(new MenuRolePermission { MenuItemId = item.Id, RoleId = roleId });
            }
            context.SaveChanges();
        }
    }
}