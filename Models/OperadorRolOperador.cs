using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("OperadorRolesOperador")]
    public class OperadorRolOperador
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OperadorId { get; set; }
        
        [ForeignKey("OperadorId")]
        public Operador Operador { get; set; } = null!;

        [Required]
        public int RolOperadorId { get; set; }
        
        [ForeignKey("RolOperadorId")]
        public RolOperador RolOperador { get; set; } = null!;

        [Display(Name = "Fecha de Asignación")]
        public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;
    }
}
