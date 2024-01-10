using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace ForoPreguntas.Models
{
    public partial  class PreguntaUsuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ID_USUARIO { get; set; }
        public int ID_PREGUNTA { get; set; }
        [ForeignKey ("ID_USUARIO")]
        public virtual Usucarrcat? Usucarrcat { get; set; }
        [ForeignKey("ID_PREGUNTA")]
        public virtual Pregunta? Pregunta { get; set; }

        public virtual ICollection<RespuestaPregunta> RespuestaPreguntas { get; set; } = new List<RespuestaPregunta>();
    }

    
    
}
