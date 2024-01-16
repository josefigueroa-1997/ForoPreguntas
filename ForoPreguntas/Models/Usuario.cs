using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ForoPreguntas.Models
{
    public partial class Usuario
    {
        public int Id { get; set; }
       
        public string Nombre { get; set; } = null!;

        public string Correo { get; set; } = null!;


        public string Contraseña { get; set; } = null!;
        public string? Salt { get; set; }
        public string? Telefono { get; set; }
        public byte[]? Imagen { get; set; }
        
    }
}
