using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ForoPreguntas.Models
{
    public partial class CarreraCategoria
    {
        [Key, Column(Order = 1)] 
        public int ID_CARRERA { get; set; }

        [Key, Column(Order = 2)] 
        public int ID_CATEGORIA { get; set; }

        public virtual Carrera? Carrera { get; set; }
        public virtual Categoria? Categoria { get; set; }

        
    }
}