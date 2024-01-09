using ForoPreguntas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Diagnostics;

namespace ForoPreguntas.Services
{
    public class Imagen
    {
        private readonly FOROPREGUNTASContext _dbcontext;
        public Imagen(FOROPREGUNTASContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public byte[] RecuperarImagen(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    byte[] bytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();


                    }
                    return bytes;
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Error al obtener la imagen: {ex.Message}");
            }

            return Array.Empty<byte>();
        }
        
    }
}
