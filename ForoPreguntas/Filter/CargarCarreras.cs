using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ForoPreguntas.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ForoPreguntas.Services;
using System.Collections;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
namespace ForoPreguntas.Filter
{
    
    public class CargarCarreras : ActionFilterAttribute
    {
        private readonly SidebarService _sidebarService;
        public CargarCarreras(SidebarService sidebarService)
        {
            _sidebarService = sidebarService;
        }
        public override void OnActionExecuting(ActionExecutingContext filtercontext)
        {  
            int? idusuario = filtercontext.HttpContext.Session.GetInt32("id");
            var carreras = _sidebarService.GetCarreras();
            var carrerausuario = idusuario.HasValue ? _sidebarService.GetCarreraUsuario(idusuario.Value) : null;
            var controller = (Controller)filtercontext.Controller;
            controller.ViewBag.Carreras = carreras; 
            controller.ViewBag.Carrerausuario = carrerausuario;     
        }
       
       

        
    }

}
