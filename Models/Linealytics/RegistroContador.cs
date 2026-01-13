using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models.Linealytics
{
    [Table("RegistrosContadores", Schema = "linealytics")]
    public class RegistroContador
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MaquinaId { get; set; }

        [Required]
        public int ContadorOK { get; set; } = 0;

        [Required]
        public int ContadorNOK { get; set; } = 0;

        [Required]
        public DateTime FechaHoraLectura { get; set; } = DateTime.UtcNow;

        public int? ModeloId { get; set; }

        // Campos calculados
        [NotMapped]
        public int TotalUnidades => ContadorOK + ContadorNOK;

        [NotMapped]
        public decimal PorcentajeCalidad => TotalUnidades > 0 ? (decimal)ContadorOK / TotalUnidades * 100 : 0;

        [NotMapped]
        public decimal PorcentajeDefectos => TotalUnidades > 0 ? (decimal)ContadorNOK / TotalUnidades * 100 : 0;

        // Relaciones
        [ForeignKey("MaquinaId")]
        public virtual Maquina Maquina { get; set; } = null!;

        [ForeignKey("ModeloId")]
        public virtual Modelo? Modelo { get; set; }
    }
}
