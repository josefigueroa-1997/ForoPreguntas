using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ForoPreguntas.Models
{
    public class Usucarrcat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        
        public int ID_USUARIO { get; set; }

        
        public int ID_CARRERA { get; set; }

       
        public int ID_CATEGORIA { get; set; }

        [ForeignKey("ID_CARRERA, ID_CATEGORIA")]
        public virtual CarreraCategoria? CarreraCategoria { get; set; }

        [ForeignKey("ID_USUARIO")]
        public virtual Usuario? Usuario { get; set; }
       
    }
}
