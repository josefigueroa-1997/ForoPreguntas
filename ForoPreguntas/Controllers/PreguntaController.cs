using Microsoft.AspNetCore.Mvc;
using ForoPreguntas.Models;
using ForoPreguntas.Filter;
using ForoPreguntas.Services;
using System.Diagnostics.Contracts;
using System.Collections;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting;
namespace ForoPreguntas.Controllers
{
    [ServiceFilter(typeof(CargarCarreras))]
    public class PreguntaController : Controller
    {
        private readonly FOROPREGUNTASContext _context;
        private readonly SidebarService _sidebarService;
        private readonly Imagen _imagen;
        private readonly PreguntaServices _preguntaservice;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PreguntaController(FOROPREGUNTASContext context, SidebarService sidebarService,Imagen imagen, PreguntaServices preguntaServices, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _sidebarService = sidebarService;
            _imagen = imagen;
            _preguntaservice = preguntaServices;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FormularioGenerarPregunta(int idusuario)
        {
           if (!UsuarioEstaAutenticado())
            {
                
                return RedirectToAction("Login", "Usuario");
            }

            Carrera carrera;
            carrera = _sidebarService.GetCarreraUsuario(idusuario);
            var categoriascarrera = _sidebarService.GetCategoriasGenerales(carrera.Id);
            ViewBag.Categorias = categoriascarrera;

            return View();
        }

        private bool UsuarioEstaAutenticado()
        {
            if (HttpContext.Session.GetInt32("id") != null)
            {
                return true;
            }
            
            return false;
        }


        [HttpPost]
        public IActionResult RegistarPregunta()
        {
            try
            {
                var imagen = Request.Form.Files["imagen"];
                byte[] imagenfile = new byte[0];
                if (imagen!= null)
                {
                    imagenfile  = _imagen.RecuperarImagen(imagen);
                }
                
                string[] categorias = Request.Form["categoria[]"];
                List<int> categoriasid = categorias.Select(int.Parse).ToList();
                int? iduser = HttpContext.Session.GetInt32("id");
                int idusuario = 0;
                var nuevapregunta = new Pregunta
                {
                    TITULO = Request.Form["titulo"],
                    DETALLE_PREGUNTA = Request.Form["detalle"],
                    IMAGEN_PREGUNTA = imagenfile.Length > 0 ? imagenfile: null,
                    FECHA_PREGUNTA = DateTime.Now,
                };
                _context.Add(nuevapregunta);
                _context.SaveChanges();
                if (iduser.HasValue)
                {
                    idusuario = iduser.Value;
                    if (idusuario > 0)
                    {
                        ExisteCategoriaUsuario(categoriasid, idusuario, nuevapregunta.Id);
                    }
                }
                TempData["PreguntaRegistrada"] = true;
                return RedirectToAction("Index","Home");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return RedirectToAction("Error", "Shared");

            }


            
        }

        private void AgregarPreguntaUsuario(List<int> categoriasid, int idusuario,int idpregunta)
        {
            try
            {
                
                var idsusuariocarcat = _context.Usucarrcats.Where(u => u.ID_USUARIO == idusuario && categoriasid.Contains(u.ID_CATEGORIA)).Select(u => u.ID).ToList();
                for (int i= 0; i < idsusuariocarcat.Count; i++)
                {
                    var nuevapreguntausuario = new PreguntaUsuario
                    {
                        ID_PREGUNTA = idpregunta,
                        ID_USUARIO = idsusuariocarcat[i],
                    };
                    _context.Add(nuevapreguntausuario);
                }
                _context.SaveChanges();
            }

            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }


        public List<Usucarrcat> Obtenercategoriasagregadas(List<int> idcategorias, int idusuario)
        {
            try
            {
                Carrera carrera = _sidebarService.GetCarreraUsuario(idusuario);

                var nuevasCategorias = idcategorias
                    .Where(idCategoria => !_context.Usucarrcats.Any(u => u.ID_USUARIO == idusuario && u.ID_CATEGORIA == idCategoria))
                    .Select(idCategoria => new Usucarrcat
                    {
                        ID_USUARIO = idusuario,
                        ID_CARRERA = carrera.Id,
                        ID_CATEGORIA = idCategoria
                    })
                    .ToList();

                return nuevasCategorias;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<Usucarrcat>();
            }
            
        }



        private void EliminarCategoriasTemporales(List<Usucarrcat> categoriastemporales)
        {
            try
            {
                if (categoriastemporales.Any())
                {
                    _context.Usucarrcats.RemoveRange(categoriastemporales);
                    _context.SaveChanges();
                   
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            
        }


        private void ExisteCategoriaUsuario(List<int> idcategorias,int idusuario,int idpregunta)
        {
            try
            {
                var nuevascategorias = Obtenercategoriasagregadas(idcategorias, idusuario);

                if (nuevascategorias.Any())
                {
                    foreach(var cat in nuevascategorias)
                    {
                        _context.Attach(cat);
                    }
                    _context.Usucarrcats.AddRange(nuevascategorias);
                    
                }
                AgregarPreguntaUsuario(idcategorias, idusuario, idpregunta);
                _context.SaveChanges();
                //EliminarCategoriasTemporales(nuevascategorias);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            
        }
        public IActionResult ObtenerImagenPregunta(int id)
        {
            var pregunta = _context.Preguntas.FirstOrDefault(p => p.Id == id);

            if (pregunta != null && pregunta.IMAGEN_PREGUNTA != null)
            {
                return File(pregunta.IMAGEN_PREGUNTA, "image/jpeg");
            }

            
            var rutaImagenPredeterminada = "~/path/to/default-image.jpg";
            var contenidoRootPath = _webHostEnvironment.ContentRootPath;
            var rutaFisicaImagenPredeterminada = System.IO.Path.Combine(contenidoRootPath, rutaImagenPredeterminada.TrimStart('~'));

            if (System.IO.File.Exists(rutaFisicaImagenPredeterminada))
            {
                return File(System.IO.File.ReadAllBytes(rutaFisicaImagenPredeterminada), "image/jpeg");
            }

            // Si no existe la imagen predeterminada, puedes devolver un código de estado 404
            return NotFound();
        }





    }
}
