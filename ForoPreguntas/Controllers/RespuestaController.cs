using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ForoPreguntas.Services;
using System.Runtime.CompilerServices;
using ForoPreguntas.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ForoPreguntas.Controllers
{
    public class RespuestaController : Controller
    {
        private readonly FOROPREGUNTASContext _context;
        private readonly PreguntaServices preguntaServices;
        private readonly Imagen _imagen;
        private readonly RespuestaService _respuestaservice;
        
        public RespuestaController(PreguntaServices preguntaservice,Imagen imagen,FOROPREGUNTASContext context,RespuestaService respuesta)
        {
            this.preguntaServices = preguntaservice;
            this._imagen = imagen;
            this._context = context;
            this._respuestaservice = respuesta;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> RespuestasPregunta(int? idpregunta)
        {
            if (idpregunta.HasValue)
            {
                var pregunta = await preguntaServices.GetPreguntaByID(idpregunta.Value);
                var respuestas = await _respuestaservice.GetRespuestasPregunta(idpregunta.Value);
                ViewBag.Pregunta = pregunta;
                ViewBag.Respuestas = respuestas;
                return View();
            }
            else
            {
                return RedirectToAction("Index,Home");
            }
            
        }

        [HttpPost]
        public IActionResult AgregarRespuesta()
        {
            var imagen = Request.Form.Files["imagen"];
            byte[] imagenfile = new byte[0];
            if (imagen != null)
            {
                imagenfile = _imagen.RecuperarImagen(imagen);
            }
            var nuevarespuesta = new Respuesta
            {
                DETALLE_RESPUESTA = Request.Form["detalle"],
                FECHA_RESPUESTA = DateTime.Now,
                IMAGEN_RESPUESTA = imagenfile.Length>0 ? imagenfile: null,

            };
            int idpregunta = int.Parse(Request.Form["idpregunta"]);
            _context.Add(nuevarespuesta);
            _context.SaveChanges();
            AgregarPreguntaRespuesta(idpregunta, nuevarespuesta.Id);
            return RedirectToAction("RespuestasPregunta", new {idpregunta = idpregunta});
        }

        private void AgregarPreguntaRespuesta(int idpregunta,int idrespuesta)
        {
            var nuevarespuestapregunta = new RespuestaPregunta
            {
                ID_PREGUNTA = idpregunta,
                ID_RESPUESTA = idrespuesta,
            };
            _context.Add(nuevarespuestapregunta);
            _context.SaveChanges();
        }

        
    }
}
