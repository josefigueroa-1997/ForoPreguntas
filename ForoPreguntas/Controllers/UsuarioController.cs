using Microsoft.AspNetCore.Mvc;
using ForoPreguntas.Models;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using Humanizer.Bytes;
using BCrypt;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace ForoPreguntas.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly FOROPREGUNTASContext _dbcontext;

        public UsuarioController(FOROPREGUNTASContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public IActionResult MostrarUsuario(int? id)
        {
            var c = GetCarreras();
            var resultado = GetUsuarios(id);
            ViewBag.Usuarios = resultado;
            ViewBag.Carreras = c;
            return View();
        }
        [HttpGet]
        public IActionResult Registro()
        {
            List<Carrera> carreras = dropdowncarreras();
            List<SelectListItem> items = listselectitemcarreras(carreras);  
            ViewBag.datos = items;
            return View();
        }
        private List<SelectListItem> listselectitemcarreras(List<Carrera> carreras)
        {
            List<SelectListItem> items;
            items = carreras.ConvertAll(
                    c =>
                    {

                        return new SelectListItem()
                        {
                            Text = c.Nombre,
                            Value = c.Id.ToString(),
                            Selected = false
                        };


                    }
                
                
                );
            return items;
        }
        private List<Carrera> dropdowncarreras()
        {
            try
            {

                List<Carrera> carreras = _dbcontext.Carreras.ToList();
               


                return carreras;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<Carrera>();
            }


        }
        [HttpPost]
        public IActionResult AgregarUsuario()
        {
            try
            {   
  
                if (ModelState.IsValid)
                {
                    string encryptedpassword = BCrypt.Net.BCrypt.HashPassword(Request.Form["contraseña"]);
                    
                    var nuevousuario = new Usuario
                    {
                        Nombre = Request.Form["nombre"],
                        Correo = Request.Form["correo"],
                        Contraseña = encryptedpassword,
                        Telefono = Request.Form["telefono"],
           
                };
                    _dbcontext.Add(nuevousuario);
                    _dbcontext.SaveChanges();
                    int carreraid = int.Parse(Request.Form["carrera"].ToString());
                    string[] categorias = Request.Form["categorias[]"];
                    List<int> idcategorias = categorias.Select(int.Parse).ToList();
                    AgregarTablaUsuCarCat(nuevousuario.Id, carreraid, idcategorias);
                    return RedirectToAction("AddProfilePicture", "Usuario",new { id = nuevousuario.Id});

                }
                else
                {
                    return RedirectToAction("Error", "Shared");
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return RedirectToAction("Error", "Shared");
            }



        }

        private void AgregarTablaUsuCarCat(int idusuario, int idcarrera,List<int> idcategorias) 
        {
            try {
                for(int i=0;i<idcategorias.Count;i++)
                {
                    var nuevousucarcat = new Usucarrcat
                    {
                        ID_USUARIO = idusuario,
                        ID_CARRERA = idcarrera,
                        ID_CATEGORIA = idcategorias[i],
                    };
                    _dbcontext.Add(nuevousucarcat);
                }
                _dbcontext.SaveChanges();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }


        }

        public IActionResult AddProfilePicture(int? id)
        {
            if (id != null)
            {
                var usuario = GetUsuarios(id);
                return View(usuario);
            }
            else
            {
                return RedirectToAction("Login","Usuario");
            }

        }
        [HttpPost]
        public IActionResult UpdateProfile()
        {
            try
            {
                int? iduser;
                if (HttpContext.Session.GetInt32("id") == null)
                {
                    iduser = ObtenerUltimoId();
                }
                else
                {
                    iduser = HttpContext.Session.GetInt32("id");
                }
                
                var usuario = _dbcontext.Usuarios.FirstOrDefault(u=>u.Id == iduser);
                if(usuario != null)
                {
                    var imagenfile = Request.Form.Files["imagen"];
                    if (imagenfile != null)
                    {
                        byte[] imagen = RecuperarImagen(imagenfile);
                        usuario.Imagen = imagen;
                        _dbcontext.SaveChanges();
                    }
                    
                }
                return RedirectToAction("MostrarUsuario", "Usuario");

            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return RedirectToAction("Error","Shared");
            }
        }
        public IActionResult Login(string correo,string contraseña)
        {
            if(!string.IsNullOrEmpty(correo) && !string.IsNullOrEmpty(contraseña))
            {
                var usuario = _dbcontext.Usuarios.FirstOrDefault(u => u.Correo == correo );
                if (usuario != null)
                {
                    bool verificar = verificarcontrasenas(contraseña,usuario.Contraseña);
                    if (verificar)
                    {
                        HttpContext.Session.SetInt32("id",usuario.Id);
                        HttpContext.Session.SetString("nombre", usuario.Nombre);
                        return RedirectToAction("MostrarUsuario", "Usuario");
                    }
                    else
                    {
                        ViewBag.Error = "La Contraseña es Incorrecta";
                        return View();
                        
                    }
                }
                else
                {
                    ViewBag.Error = "No existe un usuario con ese correo y/o contraseña";
                    return View();
                }        

            }

            return View();
        }

        public IActionResult Logout()
        {
           
            HttpContext.Session.Clear();

            
            return RedirectToAction("Index", "Home");
        }
    

    private bool verificarcontrasenas(string passuser,string passbd)
        {
            bool verificar = false;
            if(!string.IsNullOrEmpty(passuser) && !string.IsNullOrEmpty(passbd))
            {
                verificar = BCrypt.Net.BCrypt.Verify(passuser, passbd);
            }
            return verificar;
        }

        [HttpGet]
        public IActionResult ObtenerCategorias(int idCarrera)
        {
            try
            {
                List<CarreraCategoria> categorias = _dbcontext.CarreraCategorias.Where(c => c.ID_CARRERA == idCarrera).Include(c => c.Categoria).ToList();
                List<SelectListItem> categoriaslistitem = categorias.ConvertAll(

                    c =>
                    {
                        return new SelectListItem
                        {
                            Text = c.Categoria?.Nombre,
                            Value = c.ID_CATEGORIA.ToString(),
                        };


                    }


                    );
                return Json(categoriaslistitem);

            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return Json(new List<SelectListItem>());
            }

        }
        [HttpPost]
        public IActionResult VerificardisponibilidadEmail(string correo)
        {
            bool correodisponible = !_dbcontext.Usuarios.Any(u => u.Correo == correo);
            return Json(new {available = correodisponible});
        }

        public IActionResult ObtenerImagen(int id)
        {
            var usuario = _dbcontext.Usuarios.FirstOrDefault(u => u.Id == id);

            if (usuario != null && usuario.Imagen != null)
            {
                return File(usuario.Imagen, "image/jpeg"); 
            }

            return File("~/path/to/default-image.jpg", "image/jpeg");
        }
        private byte[] RecuperarImagen(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    byte[] bytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();


                    }
                    return bytes; 
                }
            }
            catch (Exception ex)
            {
                
                Debug.WriteLine($"Error al obtener la imagen: {ex.Message}");
            }

            return Array.Empty<byte>();
        }
        private List<Usuario> GetUsuarios(int? id) { 
        
            try {
                List<Usuario> usuarios;
                if (id.HasValue)
                {
                    usuarios = _dbcontext.Usuarios.Where(u => u.Id == id).Select(u => new Usuario
                    {
                        Id = u.Id,
                        Nombre = u.Nombre,
                        Correo = u.Correo,
                        Telefono = u.Telefono,
                        Imagen = u.Imagen,

                    }).ToList();
                }
                else
                {
                    usuarios = _dbcontext.Usuarios
                .Select(u => new Usuario
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Correo = u.Correo,
                    Telefono = u.Telefono,
                    Imagen = u.Imagen
                })
                .ToList();
                }
                

                return usuarios;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<Usuario>();
            }
        
        
        
        
        }
        public int ObtenerUltimoId()
        {
            var ultimoUsuario = _dbcontext.Usuarios.OrderByDescending(u => u.Id).FirstOrDefault();

            
            return ultimoUsuario != null ? ultimoUsuario.Id : 0;
        }
        private List<CarreraCategoria> GetCarreras()
        {
            List<CarreraCategoria> carreras = _dbcontext.CarreraCategorias.Select(c => new CarreraCategoria
            {
                ID_CARRERA = c.ID_CARRERA,
                ID_CATEGORIA = c.ID_CATEGORIA,

            }).ToList();

            return carreras;
        }




    }
}
