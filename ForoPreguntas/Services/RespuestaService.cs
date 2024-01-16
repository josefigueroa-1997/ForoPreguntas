using ForoPreguntas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Dynamic;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json.Nodes;

namespace ForoPreguntas.Services
{
    public class RespuestaService : IRespuestaService
    {
        private readonly FOROPREGUNTASContext _context;
        private readonly Imagen _imagen;
        public RespuestaService(FOROPREGUNTASContext context, Imagen imagen)
        {
            _context = context;
            _imagen = imagen;
        }

       public async Task<List<RespuestaPregunta>> GetRespuestasPregunta(int idpregunta)
        {
            try
            {
                List<RespuestaPregunta> respuestaPreguntas = new List<RespuestaPregunta>();
                respuestaPreguntas = await _context.RespuestaPreguntas.Where(rp => rp.ID_PREGUNTA == idpregunta)
                    .Include(r => r.Respuesta).Include(pu=>pu.PreguntaUsuario)
                    .OrderBy(rp => rp.Id).ToListAsync();

                return respuestaPreguntas;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error al traer las respuestas:{e.Message}");
                Debug.WriteLine($"StackTrace: {e.StackTrace}");
                return new List<RespuestaPregunta>();
            }


        }

        public bool AgregarRespuesta(string detalle, int idpregunta,IFormFile imagenfiles)
        {
            Boolean resultado = false;
            try
            {
                var imagen = imagenfiles;
                byte[] imagenfile = new byte[0];
                if (imagen != null)
                {
                    imagenfile = _imagen.RecuperarImagen(imagen);
                }
                var nuevarespuesta = new Respuesta
                {
                    DETALLE_RESPUESTA = detalle,
                    IMAGEN_RESPUESTA = imagenfile.Length>0 ? imagenfile : null,
                    FECHA_RESPUESTA = DateTime.Now,
                };
                _context.Add(nuevarespuesta);
                _context.SaveChanges();
                AgregarRespuestaPregunta(idpregunta,nuevarespuesta.Id);
                resultado = true;
                return resultado;
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Error en Agregar respuesta:{e.Message}");
                return resultado;
            }
            
        }

        public void AgregarRespuestaPregunta(int idpregunta, int idrespuesta)
        {
            try
            {
                var nuevarespuestapregunta = new RespuestaPregunta
                {
                    ID_PREGUNTA = idpregunta,
                    ID_RESPUESTA = idrespuesta,
                };
                _context.Add(nuevarespuestapregunta);
                _context.SaveChanges();
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Error al Agregar Respuesta a la pregunta: {e.Message}");
            }
        }
        public bool AgregarCalificacionRespuesta(int idrespuesta,int calificacion)
        {
            Boolean resultado = false;

            try
            {
                RespuestaPregunta respuesta = GetRespuestaById(idrespuesta);
                
                if (respuesta !=null)
                {
                    respuesta.CALIFICACION = calificacion;
                    _context.SaveChanges();
                    resultado = true;
                }
                
                return resultado;
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Error al ingresar una calificación en la respuesta: {e.Message}");
                return resultado;
            }

            
        }

        public RespuestaPregunta GetRespuestaById(int idrespuesta)
        {

            RespuestaPregunta respuesta = _context.RespuestaPreguntas.Where(r => r.Id == idrespuesta).FirstOrDefault();
            if (respuesta == null)
            {
               
                throw new InvalidOperationException("Response not found for the given ID");
            }
            return respuesta;
        }


    }
}
