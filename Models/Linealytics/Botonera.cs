using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models.Linealytics
{
    [Table("Botoneras", Schema = "linealytics")]
    public class Botonera
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la botonera es requerido")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La dirección IP es requerida")]
        [StringLength(50)]
        [Display(Name = "Dirección IP")]
        public string DireccionIP { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Número de Serie")]
        public string? NumeroSerie { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación con Maquina (muchos a uno)
        [Required(ErrorMessage = "La máquina es requerida")]
        [Display(Name = "Máquina")]
        public int MaquinaId { get; set; }

        [ForeignKey("MaquinaId")]
        public virtual Maquina Maquina { get; set; } = null!;
    }
}
