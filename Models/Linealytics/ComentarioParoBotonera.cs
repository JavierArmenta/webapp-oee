using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models.Linealytics
{
    [Table("ComentariosParoBotonera", Schema = "linealytics")]
    public class ComentarioParoBotonera
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RegistroParoBotoneraId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Comentario { get; set; } = string.Empty;

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("RegistroParoBotoneraId")]
        public virtual RegistroParoBotonera RegistroParoBotonera { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
