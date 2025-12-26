using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("Operadores")]
    public class Operador
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100)]
        public string Apellido { get; set; }= string.Empty;

        [Required(ErrorMessage = "El número de empleado es requerido")]
        [StringLength(50)]
        [Display(Name = "Número de Empleado")]
        public string NumeroEmpleado { get; set; }= string.Empty;

        [StringLength(255)]
        [Display(Name = "Código PIN")]
        public string CodigoPinHashed { get; set; }= string.Empty;

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        // Propiedad calculada para nombre completo
        [NotMapped]
        public string NombreCompleto => $"{Nombre} {Apellido}";

        // Relación muchos a muchos con RolesOperador
        public ICollection<OperadorRolOperador> OperadorRoles { get; set; } = new List<OperadorRolOperador>();
    }
}