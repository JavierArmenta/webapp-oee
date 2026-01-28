using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models.Linealytics
{
    /// <summary>
    /// Registros de fallas detectadas - Sistema independiente de paros
    /// Los registros se crean automáticamente desde API cuando un dispositivo detecta una falla
    /// </summary>
    [Table("RegistrosFallas", Schema = "linealytics")]
    public class RegistroFalla
    {
        [Key]
        public int Id { get; set; }

        // Referencias básicas (desde API)
        [Required]
        [Display(Name = "Tipo de Falla")]
        public int CatalogoFallaId { get; set; }

        [Required]
        [Display(Name = "Máquina")]
        public int MaquinaId { get; set; }

        [Required]
        [Display(Name = "Fecha/Hora Detección")]
        public DateTime FechaHoraDeteccion { get; set; } = DateTime.UtcNow;

        // Gestión posterior (desde Web)
        [MaxLength(20)]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, EnAtencion, Resuelta

        [Display(Name = "Técnico Asignado")]
        public int? TecnicoAsignadoId { get; set; }

        [Display(Name = "Fecha/Hora Atención")]
        public DateTime? FechaHoraAtencion { get; set; }

        [Display(Name = "Fecha/Hora Resolución")]
        public DateTime? FechaHoraResolucion { get; set; }

        [Display(Name = "Duración (minutos)")]
        public int? DuracionMinutos { get; set; }

        [MaxLength(2000)]
        [Display(Name = "Acciones Tomadas")]
        public string? AccionesTomadas { get; set; }

        [Display(Name = "Fecha Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relaciones
        [ForeignKey("CatalogoFallaId")]
        public virtual CatalogoFalla CatalogoFalla { get; set; } = null!;

        [ForeignKey("MaquinaId")]
        public virtual Maquina Maquina { get; set; } = null!;

        [ForeignKey("TecnicoAsignadoId")]
        public virtual Operador? TecnicoAsignado { get; set; }
    }
}
