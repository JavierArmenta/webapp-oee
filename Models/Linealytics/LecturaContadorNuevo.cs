using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models.Linealytics
{
    [Table("LecturasContadorNuevo", Schema = "linealytics")]
    public class LecturaContadorNuevo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Corrida")]
        public int CorridaId { get; set; }

        [Required]
        [Display(Name = "Contador Dispositivo")]
        public int ContadorDispositivoId { get; set; }

        [Display(Name = "Producto")]
        public int? ProductoId { get; set; }

        [Display(Name = "Valor del Contador")]
        public long ContadorValor { get; set; }

        [Display(Name = "Valor Anterior")]
        public long? ContadorAnterior { get; set; }

        [Display(Name = "Diferencia")]
        public long Diferencia { get; set; }

        [Display(Name = "Producción Incremental")]
        public long ProduccionIncremental { get; set; }

        [Display(Name = "Es Reset")]
        public bool EsReset { get; set; }

        [Display(Name = "Es Ruido")]
        public bool EsRuido { get; set; }

        [Required]
        [Display(Name = "Fecha y Hora")]
        public DateTime FechaHoraLectura { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("CorridaId")]
        public virtual CorridaProduccion? Corrida { get; set; }

        [ForeignKey("ContadorDispositivoId")]
        public virtual ContadorDispositivo? ContadorDispositivo { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; }
    }
}
