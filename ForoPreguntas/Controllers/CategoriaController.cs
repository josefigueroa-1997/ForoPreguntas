using Microsoft.AspNetCore.Mvc;
using ForoPreguntas.Services;
using System.Diagnostics;

namespace ForoPreguntas.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly SidebarService _sidebarService;

        public CategoriaController(SidebarService sidebarService)
        {
            _sidebarService = sidebarService;
        }

        [HttpGet]
        public IActionResult ObtenerCategorias(int id)
        {
            try
            {
                var categorias = _sidebarService.GetCategorias(id);
                return Json(categorias);

            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return NotFound();
            }
            
        }
    }
}
