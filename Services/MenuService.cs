using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Services
{
    public interface IMenuService
    {
        Task<List<MenuItem>> GetMenusForUserAsync(string userId);
        Task<List<MenuItem>> GetAllMenusAsync();
        Task<List<MenuItem>> GetParentMenusAsync();
        Task<MenuItem?> GetMenuByIdAsync(int id);
        Task<bool> CreateMenuAsync(MenuItem menu);
        Task<bool> UpdateMenuAsync(MenuItem menu);
        Task<bool> DeleteMenuAsync(int id);
        Task<List<MenuRolePermission>> GetPermissionsForMenuAsync(int menuId);
        Task<bool> UpdatePermissionsAsync(int menuId, List<string> roleIds);
    }

    public class MenuService : IMenuService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MenuService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Obtiene los menús permitidos para un usuario basado en sus roles
        /// </summary>
        public async Task<List<MenuItem>> GetMenusForUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<MenuItem>();

            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Any()) return new List<MenuItem>();

            // Obtener IDs de roles del usuario
            var roleIds = await _context.Roles
                .Where(r => userRoles.Contains(r.Name!))
                .Select(r => r.Id)
                .ToListAsync();

            // Obtener menús padre permitidos para los roles del usuario
            var allowedMenuIds = await _context.MenuRolePermissions
                .Where(p => roleIds.Contains(p.RoleId))
                .Select(p => p.MenuItemId)
                .Distinct()
                .ToListAsync();

            // Obtener menús padre con sus hijos
            var menus = await _context.MenuItems
                .Where(m => m.Activo && m.ParentId == null && allowedMenuIds.Contains(m.Id))
                .Include(m => m.Children.Where(c => c.Activo && allowedMenuIds.Contains(c.Id)))
                .OrderBy(m => m.Orden)
                .ToListAsync();

            // Ordenar hijos
            foreach (var menu in menus)
            {
                menu.Children = menu.Children.OrderBy(c => c.Orden).ToList();
            }

            return menus;
        }

        /// <summary>
        /// Obtiene todos los menús (para administración)
        /// </summary>
        public async Task<List<MenuItem>> GetAllMenusAsync()
        {
            return await _context.MenuItems
                .Include(m => m.Parent)
                .Include(m => m.Children)
                .OrderBy(m => m.ParentId == null ? 0 : 1)
                .ThenBy(m => m.Orden)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene solo menús padre (para dropdown)
        /// </summary>
        public async Task<List<MenuItem>> GetParentMenusAsync()
        {
            return await _context.MenuItems
                .Where(m => m.ParentId == null)
                .OrderBy(m => m.Orden)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene un menú por ID
        /// </summary>
        public async Task<MenuItem?> GetMenuByIdAsync(int id)
        {
            return await _context.MenuItems
                .Include(m => m.Parent)
                .Include(m => m.Children)
                .Include(m => m.MenuRolePermissions)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        /// <summary>
        /// Crea un nuevo menú
        /// </summary>
        public async Task<bool> CreateMenuAsync(MenuItem menu)
        {
            try
            {
                _context.MenuItems.Add(menu);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Actualiza un menú existente
        /// </summary>
        public async Task<bool> UpdateMenuAsync(MenuItem menu)
        {
            try
            {
                _context.MenuItems.Update(menu);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Elimina un menú
        /// </summary>
        public async Task<bool> DeleteMenuAsync(int id)
        {
            try
            {
                var menu = await _context.MenuItems
                    .Include(m => m.Children)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (menu == null) return false;

                // No eliminar si tiene hijos
                if (menu.Children.Any()) return false;

                _context.MenuItems.Remove(menu);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene los permisos de un menú
        /// </summary>
        public async Task<List<MenuRolePermission>> GetPermissionsForMenuAsync(int menuId)
        {
            return await _context.MenuRolePermissions
                .Where(p => p.MenuItemId == menuId)
                .Include(p => p.Role)
                .ToListAsync();
        }

        /// <summary>
        /// Actualiza los permisos de un menú
        /// </summary>
        public async Task<bool> UpdatePermissionsAsync(int menuId, List<string> roleIds)
        {
            try
            {
                // Eliminar permisos existentes
                var existingPermissions = await _context.MenuRolePermissions
                    .Where(p => p.MenuItemId == menuId)
                    .ToListAsync();

                _context.MenuRolePermissions.RemoveRange(existingPermissions);

                // Agregar nuevos permisos
                foreach (var roleId in roleIds)
                {
                    _context.MenuRolePermissions.Add(new MenuRolePermission
                    {
                        MenuItemId = menuId,
                        RoleId = roleId
                    });
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
