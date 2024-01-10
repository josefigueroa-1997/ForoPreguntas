using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ForoPreguntas.Models
{
    public class Respuesta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string? DETALLE_RESPUESTA { get; set; }
        public byte[]? IMAGEN_RESPUESTA { get; set; }
        public DateTime? FECHA_RESPUESTA { get; set; }

        public ICollection<RespuestaPregunta> RespuestaPreguntas { get; set; } = new List<RespuestaPregunta>();
    }
}
