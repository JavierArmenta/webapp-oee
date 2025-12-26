using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("RolesOperador")]
    public class RolOperador
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del rol es requerido")]
        [StringLength(100)]
        [Display(Name = "Nombre del Rol")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación muchos a muchos con Operadores
        public ICollection<OperadorRolOperador> OperadorRoles { get; set; } = new List<OperadorRolOperador>();
    }
}
