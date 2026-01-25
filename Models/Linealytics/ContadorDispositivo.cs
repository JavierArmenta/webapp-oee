using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models.Linealytics
{
    [Table("ContadoresDispositivo", Schema = "linealytics")]
    public class ContadorDispositivo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Máquina")]
        public int MaquinaId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Tipo de Contador")]
        public string TipoContador { get; set; } = "Produccion"; // Produccion, Ciclos, Defectos

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("MaquinaId")]
        public virtual Maquina? Maquina { get; set; }

        public virtual ICollection<CorridaProduccion> Corridas { get; set; } = new List<CorridaProduccion>();
    }
}
