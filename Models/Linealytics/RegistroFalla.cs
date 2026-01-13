using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models.Linealytics
{
    [Table("RegistrosFallas", Schema = "linealytics")]
    public class RegistroFalla
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FallaId { get; set; }

        [Required]
        public int MaquinaId { get; set; }

        [Required]
        public DateTime FechaHoraLectura { get; set; } = DateTime.UtcNow;

        public int? ModeloId { get; set; }

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        // Relaciones
        [ForeignKey("FallaId")]
        public virtual CausaParo CausaParo { get; set; } = null!;

        [ForeignKey("MaquinaId")]
        public virtual Maquina Maquina { get; set; } = null!;

        [ForeignKey("ModeloId")]
        public virtual Modelo? Modelo { get; set; }
    }
}
