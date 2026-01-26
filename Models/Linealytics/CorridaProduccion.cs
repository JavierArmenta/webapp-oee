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
        [Display(Name = "Máquina")]
        public int MaquinaId { get; set; }

        [Required]
        [Display(Name = "Producto")]
        public int ProductoId { get; set; }

        [Required]
        [Display(Name = "Fecha Inicio")]
        public DateTime FechaInicio { get; set; } = DateTime.UtcNow;

        [Display(Name = "Fecha Fin")]
        public DateTime? FechaFin { get; set; }

        // Contadores OK
        [Display(Name = "Contador OK Inicial")]
        public long ContadorOKInicial { get; set; }

        [Display(Name = "Contador OK Final")]
        public long ContadorOKFinal { get; set; }

        [Display(Name = "Último Contador OK")]
        public long UltimoContadorOK { get; set; }

        [Display(Name = "Producción OK")]
        public long ProduccionOK { get; set; }

        [Display(Name = "Resets OK")]
        public int NumeroResetsOK { get; set; }

        // Contadores NOK
        [Display(Name = "Contador NOK Inicial")]
        public long ContadorNOKInicial { get; set; }

        [Display(Name = "Contador NOK Final")]
        public long ContadorNOKFinal { get; set; }

        [Display(Name = "Último Contador NOK")]
        public long UltimoContadorNOK { get; set; }

        [Display(Name = "Producción NOK")]
        public long ProduccionNOK { get; set; }

        [Display(Name = "Resets NOK")]
        public int NumeroResetsNOK { get; set; }

        // Metadatos
        [Display(Name = "Número de Lecturas")]
        public int NumeroLecturas { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Activa"; // Activa, Cerrada

        // Navegación
        [ForeignKey("MaquinaId")]
        public virtual Maquina? Maquina { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; }

        public virtual ICollection<LecturaContador> Lecturas { get; set; } = new List<LecturaContador>();
    }
}
