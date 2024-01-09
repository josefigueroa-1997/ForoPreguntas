using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ForoPreguntas.Controllers
{
    public class RespuestaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> RespuestasPregunta(int? idpregunta)
        {
            if (idpregunta.HasValue)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index,Home");
            }
            
        }
    }
}
