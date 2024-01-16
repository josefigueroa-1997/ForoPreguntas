using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ForoPreguntas.Models
{
    public class RespuestaPregunta
    {
        [Key]
        public int Id { get; set; }
        public int ID_PREGUNTA { get; set; } 
        public int ID_RESPUESTA { get; set; }
       public int? CALIFICACION { get; set; }

        [ForeignKey("ID_PREGUNTA")]
        public virtual PreguntaUsuario? PreguntaUsuario { get; set; }
        [ForeignKey("ID_RESPUESTA")]
        public virtual Respuesta? Respuesta { get; set; }

    }
}
