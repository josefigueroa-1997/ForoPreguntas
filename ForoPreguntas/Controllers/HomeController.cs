using ForoPreguntas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ForoPreguntas.Filter;
using ForoPreguntas.Services;
namespace ForoPreguntas.Controllers
{
    [ServiceFilter(typeof(CargarCarreras))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SidebarService _sideBarService; 

        public HomeController(ILogger<HomeController> logger, SidebarService sideBarService)
        {
            _logger = logger;
            _sideBarService = sideBarService; 
        }


        public IActionResult Index()
        {
            return View();
        }
       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}