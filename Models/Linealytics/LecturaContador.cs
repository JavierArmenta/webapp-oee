using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models.Linealytics
{
    [Table("LecturasContador", Schema = "linealytics")]
    public class LecturaContador
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Corrida")]
        public int CorridaId { get; set; }

        [Required]
        [Display(Name = "Máquina")]
        public int MaquinaId { get; set; }

        [Required]
        [Display(Name = "Producto")]
        public int ProductoId { get; set; }

        // Contador OK
        [Display(Name = "Contador OK")]
        public long ContadorOK { get; set; }

        [Display(Name = "Contador OK Anterior")]
        public long? ContadorOKAnterior { get; set; }

        [Display(Name = "Diferencia OK")]
        public long DiferenciaOK { get; set; }

        [Display(Name = "Producción OK")]
        public long ProduccionOK { get; set; }

        [Display(Name = "Es Reset OK")]
        public bool EsResetOK { get; set; }

        // Contador NOK
        [Display(Name = "Contador NOK")]
        public long ContadorNOK { get; set; }

        [Display(Name = "Contador NOK Anterior")]
        public long? ContadorNOKAnterior { get; set; }

        [Display(Name = "Diferencia NOK")]
        public long DiferenciaNOK { get; set; }

        [Display(Name = "Producción NOK")]
        public long ProduccionNOK { get; set; }

        [Display(Name = "Es Reset NOK")]
        public bool EsResetNOK { get; set; }

        [Required]
        [Display(Name = "Fecha y Hora")]
        public DateTime FechaHoraLectura { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("CorridaId")]
        public virtual CorridaProduccion? Corrida { get; set; }

        [ForeignKey("MaquinaId")]
        public virtual Maquina? Maquina { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; }
    }
}
