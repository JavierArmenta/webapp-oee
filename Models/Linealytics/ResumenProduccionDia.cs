using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models.Linealytics
{
    [Table("ResumenesProduccionDia", Schema = "linealytics")]
    public class ResumenProduccionDia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Contador Dispositivo")]
        public int ContadorDispositivoId { get; set; }

        [Display(Name = "Producto")]
        public int? ProductoId { get; set; }

        [Required]
        [Display(Name = "Fecha")]
        public DateOnly Fecha { get; set; }

        [Display(Name = "Producción Total")]
        public long ProduccionTotal { get; set; }

        [Display(Name = "Número de Lecturas")]
        public int NumeroLecturas { get; set; }

        [Display(Name = "Número de Resets")]
        public int NumeroResets { get; set; }

        [Display(Name = "Corridas Iniciadas")]
        public int NumeroCorridasIniciadas { get; set; }

        [Display(Name = "Corridas Cerradas")]
        public int NumeroCorridasCerradas { get; set; }

        [Display(Name = "Tiempo de Producción (min)")]
        public int TiempoProduccionMinutos { get; set; }

        // Navegación
        [ForeignKey("ContadorDispositivoId")]
        public virtual ContadorDispositivo? ContadorDispositivo { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; }
    }
}
