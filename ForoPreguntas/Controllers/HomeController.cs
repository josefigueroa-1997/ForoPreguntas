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
        private readonly FOROPREGUNTASContext _dbcontext;
        private readonly PreguntaServices _preguntaServices;
        public HomeController(ILogger<HomeController> logger, SidebarService sideBarService, FOROPREGUNTASContext dbcontext, PreguntaServices preguntaServices)
        {
            _logger = logger;
            _sideBarService = sideBarService;
            _dbcontext = dbcontext;
            _preguntaServices = preguntaServices;
        }


        public IActionResult Index(int? idpregunta, int? idcarrera, int? idcategoria, int? idusuario)
        {
            var preguntasusuario = _preguntaServices.GetPreguntaUsuarios(idpregunta, idcarrera, idcategoria, idusuario);
            ViewBag.Preguntasusuario = preguntasusuario;
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