using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("DepartamentosOperador")]
    public class DepartamentoOperador
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del departamento es requerido")]
        [StringLength(100)]
        [Display(Name = "Nombre del Departamento")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [StringLength(7)]
        [Display(Name = "Color")]
        public string? Color { get; set; } = "#666666";

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación muchos a muchos con Operadores
        public ICollection<OperadorDepartamento> OperadorDepartamentos { get; set; } = new List<OperadorDepartamento>();
    }
}
