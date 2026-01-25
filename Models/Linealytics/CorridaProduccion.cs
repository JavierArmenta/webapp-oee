using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models.Linealytics
{
    [Table("CorridasProduccion", Schema = "linealytics")]
    public class CorridaProduccion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Contador Dispositivo")]
        public int ContadorDispositivoId { get; set; }

        [Display(Name = "Producto")]
        public int? ProductoId { get; set; }

        [Required]
        [Display(Name = "Fecha Inicio")]
        public DateTime FechaInicio { get; set; } = DateTime.UtcNow;

        [Display(Name = "Fecha Fin")]
        public DateTime? FechaFin { get; set; }

        [Display(Name = "Contador Inicial")]
        public long ContadorInicial { get; set; }

        [Display(Name = "Contador Final")]
        public long ContadorFinal { get; set; }

        [Display(Name = "Último Valor")]
        public long UltimoContadorValor { get; set; }

        [Display(Name = "Producción Total")]
        public long ProduccionTotal { get; set; }

        [Display(Name = "Número de Resets")]
        public int NumeroResets { get; set; }

        [Display(Name = "Número de Lecturas")]
        public int NumeroLecturas { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Activa"; // Activa, Cerrada

        // Navegación
        [ForeignKey("ContadorDispositivoId")]
        public virtual ContadorDispositivo? ContadorDispositivo { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; }

        public virtual ICollection<LecturaContadorNuevo> Lecturas { get; set; } = new List<LecturaContadorNuevo>();
    }
}
