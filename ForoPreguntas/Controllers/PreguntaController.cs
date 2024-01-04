using Microsoft.AspNetCore.Mvc;
using ForoPreguntas.Models;
using ForoPreguntas.Filter;
using ForoPreguntas.Services;
using System.Diagnostics.Contracts;
using System.Collections;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;

namespace ForoPreguntas.Controllers
{
    [ServiceFilter(typeof(CargarCarreras))]
    public class PreguntaController : Controller
    {
        private readonly FOROPREGUNTASContext _context;
        private readonly SidebarService _sidebarService;
        private readonly Imagen _imagen;
        public PreguntaController(FOROPREGUNTASContext context, SidebarService sidebarService,Imagen imagen)
        {
            _context = context;
            _sidebarService = sidebarService;
            _imagen = imagen;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FormularioGenerarPregunta(int idusuario)
        {
            Carrera carrera;
            carrera = _sidebarService.GetCarreraUsuario(idusuario);
            var categoriascarrera = _sidebarService.GetCategoriasGenerales(carrera.Id);
            ViewBag.Categorias = categoriascarrera;
            return View();
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
                    imagenfile = _imagen.RecuperarImagen(imagen);
                }
                string[] categorias = Request.Form["categoria[]"];
                List<int> categoriasid = categorias.Select(int.Parse).ToList();
                int? iduser = HttpContext.Session.GetInt32("id");
                int idusuario = 0;
                var nuevapregunta = new Pregunta
                {
                    TITULO = Request.Form["titulo"],
                    DETALLE_PREGUNTA = Request.Form["detalle"],
                    IMAGEN_PREGUNTA = imagenfile,
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
               
                return RedirectToAction("Index","Home");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return RedirectToAction("Error", "Shared");

            }


            
        }

        private void AgregarPreguntaUsuario(List<int> categoriasid, int idpregunta,int? idusuario)
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
                    int idtemporal = 62;
                    foreach(var categoria in categoriastemporales)
                    {
                        categoria.ID_USUARIO = idtemporal;
                       
                    }
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
                AgregarPreguntaUsuario(idcategorias, idpregunta, idusuario);
                _context.SaveChanges();
                EliminarCategoriasTemporales(nuevascategorias);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            
        }

       

        
    }
}
