using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForoPreguntas.Models
{
    public partial class Pregunta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? DETALLE_PREGUNTA { get; set; }
        public byte[]? IMAGEN_PREGUNTA { get; set; }
        public DateTime? FECHA_PREGUNTA { get; set; }
        public string? TITULO { get; set; }

        public virtual ICollection<PreguntaUsuario> PreguntaUsuarios { get; set; } = new List<PreguntaUsuario>();
    }
}
