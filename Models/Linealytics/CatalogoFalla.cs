using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models.Linealytics
{
    /// <summary>
    /// Catálogo maestro de tipos de fallas - Sistema independiente de paros
    /// </summary>
    [Table("CatalogoFallas", Schema = "linealytics")]
    public class CatalogoFalla
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es requerido")]
        [MaxLength(50)]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(200)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [MaxLength(20)]
        [Display(Name = "Severidad")]
        public string? Severidad { get; set; } // Baja, Media, Alta, Crítica

        [MaxLength(50)]
        [Display(Name = "Categoría")]
        public string? Categoria { get; set; } // Mecánica, Eléctrica, Neumática, Software, Hidráulica, Otra

        [Display(Name = "Tiempo Estimado Solución (minutos)")]
        public int? TiempoEstimadoSolucionMinutos { get; set; }

        [MaxLength(7)]
        [Display(Name = "Color")]
        public string? Color { get; set; } // Para visualización en gráficas (#RRGGBB)

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Display(Name = "Fecha de Modificación")]
        public DateTime? FechaModificacion { get; set; }

        // Relación con registros de fallas
        public virtual ICollection<RegistroFalla> RegistrosFallas { get; set; } = new List<RegistroFalla>();
    }
}
