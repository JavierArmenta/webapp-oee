using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models.Linealytics
{
    [Table("RegistrosParoBotonera", Schema = "linealytics")]
    public class RegistroParoBotonera
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MaquinaId { get; set; }

        [Required]
        public int DepartamentoId { get; set; }

        public int? OperadorId { get; set; }

        public int? BotonId { get; set; }

        [Required]
        public int BotoneraId { get; set; }

        [Required]
        public DateTime FechaHoraInicio { get; set; } = DateTime.UtcNow;

        public DateTime? FechaHoraFin { get; set; }

        public int? DuracionMinutos { get; set; }

        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "Abierto";

        // Relaciones
        [ForeignKey("MaquinaId")]
        public virtual Maquina Maquina { get; set; } = null!;

        [ForeignKey("DepartamentoId")]
        public virtual DepartamentoOperador DepartamentoOperador { get; set; } = null!;

        [ForeignKey("OperadorId")]
        public virtual Operador? Operador { get; set; }

        [ForeignKey("BotonId")]
        public virtual Boton? Boton { get; set; }

        [ForeignKey("BotoneraId")]
        public virtual Botonera Botonera { get; set; } = null!;

        // Colección de comentarios
        public virtual ICollection<ComentarioParoBotonera> Comentarios { get; set; } = new List<ComentarioParoBotonera>();
    }
}
