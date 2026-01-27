using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "URL")]
        [StringLength(200)]
        public string? Url { get; set; }

        [Display(Name = "Icono")]
        [StringLength(50)]
        public string? Icono { get; set; }

        [Display(Name = "Menú Padre")]
        public int? ParentId { get; set; }

        [Display(Name = "Orden")]
        public int Orden { get; set; } = 0;

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public MenuItem? Parent { get; set; }
        public ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();
        public ICollection<MenuRolePermission> MenuRolePermissions { get; set; } = new List<MenuRolePermission>();
    }
}
