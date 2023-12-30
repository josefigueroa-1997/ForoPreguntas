using Microsoft.AspNetCore.Mvc;
using ForoPreguntas.Services;
using System.Diagnostics;
using ForoPreguntas.Models;

namespace ForoPreguntas.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly SidebarService _sidebarService;

        public CategoriaController(SidebarService sidebarService)
        {
            _sidebarService = sidebarService;
        }

        public IActionResult ObtenerCategorias(int idcarrera, int? idusuario)
        {
            try
            {
                List<Categoria> categorias;

                if (idusuario != null)
                {
                    categorias = _sidebarService.GetCategoriasUsuario(idcarrera, idusuario.Value);
                    
                }
                else
                {
                    categorias = _sidebarService.GetCategoriasGenerales(idcarrera);
                    
                }

                return Json(categorias);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error en ObtenerCategorias: {e.Message}");
                return Json(new { error = "Ocurrió un error al obtener las categorías." });
            }
        }
    }
}
