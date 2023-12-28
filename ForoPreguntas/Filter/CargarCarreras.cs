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
            var carreras = _sidebarService.GetCarreras();          
            var controller = (Controller)filtercontext.Controller;
            controller.ViewBag.Carreras = carreras; 
               
        }
       
       

        
    }

}
