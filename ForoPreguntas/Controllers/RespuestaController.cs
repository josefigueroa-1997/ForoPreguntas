using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ForoPreguntas.Services;
using System.Runtime.CompilerServices;
using ForoPreguntas.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
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
        public IActionResult AgregarRespuesta(string detalle,string idpregunta,IFormFile imagen)
        {
            
            int idpreguntas = int.Parse(idpregunta);
            Boolean resultado = _respuestaservice.AgregarRespuesta(detalle, idpreguntas, imagen);
            if (resultado)
            {
                return RedirectToAction("RespuestasPregunta", new { idpregunta = idpreguntas });
            }
            else
            {
                return RedirectToAction("Error", "Shared");
            }
            
        }

        [HttpPost]
        public IActionResult AgregarCalificacionRespuesta(string idrespuesta,string calificacion)
        {
            int idrespuestas = int.Parse(idrespuesta);
            int calif = int.Parse(calificacion);
            Boolean resultado = _respuestaservice.AgregarCalificacionRespuesta(idrespuestas, calif);
            if (resultado)
            {
                RespuestaPregunta idpreguntas = _respuestaservice.GetRespuestaById(idrespuestas);
                return RedirectToAction("RespuestasPregunta", new {idpregunta = idpreguntas.ID_PREGUNTA});
            }
            else
            {
                return RedirectToAction("Error","Shared");
            }
        }
       
        
    }
}
