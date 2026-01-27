using Microsoft.AspNetCore.Identity;

namespace WebApp.Models
{
    public class MenuRolePermission
    {
        public int Id { get; set; }

        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; } = null!;

        public string RoleId { get; set; } = string.Empty;
        public IdentityRole Role { get; set; } = null!;

        public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;
    }
}
