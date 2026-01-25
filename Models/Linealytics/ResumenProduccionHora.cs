using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models.Linealytics
{
    [Table("ResumenesProduccionHora", Schema = "linealytics")]
    public class ResumenProduccionHora
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

        [Required]
        [Display(Name = "Hora")]
        public int Hora { get; set; } // 0-23

        [Display(Name = "Producción Total")]
        public long ProduccionTotal { get; set; }

        [Display(Name = "Número de Lecturas")]
        public int NumeroLecturas { get; set; }

        [Display(Name = "Número de Resets")]
        public int NumeroResets { get; set; }

        [Display(Name = "Contador Inicio")]
        public long ContadorInicio { get; set; }

        [Display(Name = "Contador Fin")]
        public long ContadorFin { get; set; }

        [Display(Name = "Valor Mínimo")]
        public long ValorMinimo { get; set; }

        [Display(Name = "Valor Máximo")]
        public long ValorMaximo { get; set; }

        // Navegación
        [ForeignKey("ContadorDispositivoId")]
        public virtual ContadorDispositivo? ContadorDispositivo { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; }
    }
}
