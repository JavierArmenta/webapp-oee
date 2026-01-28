using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class MenuItemViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Icono (clase CSS)")]
        [StringLength(50)]
        public string? Icono { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }

    public class MenuPermissionViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool Selected { get; set; }
    }

    public class ManageMenuPermissionsViewModel
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public List<MenuPermissionViewModel> Permissions { get; set; } = new();
    }
}
