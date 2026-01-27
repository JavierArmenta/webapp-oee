using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models.Linealytics
{
    [Table("RegistrosParos", Schema = "linealytics")]
    public class RegistroParo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MaquinaId { get; set; }

        public int? MetricasMaquinaId { get; set; }

        public int? CausaParoId { get; set; }

        [Required]
        public DateTime FechaHoraInicio { get; set; }

        public DateTime? FechaHoraFin { get; set; }

        public int? DuracionMinutos { get; set; }

        public int? OperadorResponsableId { get; set; }

        public int? OperadorSolucionaId { get; set; }

        [MaxLength(1000)]
        public string? Descripcion { get; set; }

        [MaxLength(1000)]
        public string? Solucion { get; set; }

        public bool EsMicroParo { get; set; } = false;

        [MaxLength(20)]
        public string Estado { get; set; } = "Abierto"; // Abierto, Cerrado, EnAtencion

        public DateTime? FechaAtencion { get; set; }

        public DateTime? FechaCierre { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaModificacion { get; set; }

        // Relaciones
        [ForeignKey("MaquinaId")]
        public virtual Maquina Maquina { get; set; } = null!;

        [ForeignKey("MetricasMaquinaId")]
        public virtual MetricasMaquina? MetricasMaquina { get; set; }

        [ForeignKey("OperadorResponsableId")]
        public virtual Operador? OperadorResponsable { get; set; }

        [ForeignKey("OperadorSolucionaId")]
        public virtual Operador? OperadorSoluciona { get; set; }

        public virtual ICollection<HistorialCambioParo> HistorialCambios { get; set; } = new List<HistorialCambioParo>();
    }
}
