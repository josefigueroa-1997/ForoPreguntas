﻿using Microsoft.AspNetCore.Mvc;
using ForoPreguntas.Models;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using Humanizer.Bytes;
using BCrypt;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using ForoPreguntas.Filter;
using ForoPreguntas.Services;
using System.Runtime.CompilerServices;

namespace ForoPreguntas.Controllers
{
    [ServiceFilter(typeof(CargarCarreras))]
    public class UsuarioController : Controller
    {
        private readonly FOROPREGUNTASContext _dbcontext;
        private readonly SidebarService _sidebarservice;
        private readonly Imagen _imagen;
        private readonly PreguntaServices _preguntaservices;
        public UsuarioController(FOROPREGUNTASContext dbcontext, SidebarService sidebarservice, Imagen imagen, PreguntaServices preguntaservices)
        {
            _dbcontext = dbcontext;
            _sidebarservice = sidebarservice;
            _imagen = imagen;
            _preguntaservices = preguntaservices;
        }

        public IActionResult MostrarUsuario(int? id)
        {
            if (id.HasValue)
            {
                var resultado = GetUsuarios(id);
                ViewBag.Usuarios = resultado;
                return View();
            }
            else
            {

                return RedirectToAction("Login", "Usuario");
            }

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
                    return RedirectToAction("AddProfilePicture", "Usuario", new { id = nuevousuario.Id });

                }
                else
                {
                    return RedirectToAction("Error", "Shared");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return RedirectToAction("Error", "Shared");
            }



        }

        private void AgregarTablaUsuCarCat(int idusuario, int idcarrera, List<int> idcategorias)
        {
            try {
                for (int i = 0; i < idcategorias.Count; i++)
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
            catch (Exception e)
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
                return RedirectToAction("Login", "Usuario");
            }

        }

        public IActionResult ActualizarDatos(int? id)
        {
            if (id.HasValue)
            {
                var datosusuario = GetUsuarios(id);
                return View(datosusuario);
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
            
        }

        

        [HttpPost]
        public IActionResult UpdateProfile()
        {
            try
            {
                 int? iduser;
                 
                if(HttpContext.Session.GetInt32("id") == null)
                {
                    iduser = ObtenerUltimoId();
                    var usuario = _dbcontext.Usuarios.FirstOrDefault(u => u.Id == iduser);
                    if (usuario != null)
                    {
                        var imagenfile = Request.Form.Files["imagen"];
                        if (imagenfile != null)
                        {
                            byte[] imagen = _imagen.RecuperarImagen(imagenfile) ;

                            usuario.Imagen = imagen;

                        }
                        _dbcontext.SaveChanges();
                        HttpContext.Session.SetInt32("id", usuario.Id);
                        HttpContext.Session.SetString("nombre", usuario.Nombre);
                        return RedirectToAction("Index","Home");
                    }


                }
                else
                {
                    iduser = HttpContext.Session.GetInt32("id");
                    var usuario = _dbcontext.Usuarios.FirstOrDefault(u => u.Id == iduser);
                    if (usuario != null)
                    {



                        var imagenfile = Request.Form.Files["imagen"];
                        if (imagenfile != null)
                        {
                            byte[] imagen = _imagen.RecuperarImagen(imagenfile);

                            usuario.Imagen = imagen;

                        }
                        usuario.Nombre = Request.Form["nombre"];
                        usuario.Correo = Request.Form["correo"];
                        usuario.Telefono = Request.Form["telefono"];
                        _dbcontext.SaveChanges();
                    }
                    return RedirectToAction("MostrarUsuario", "Usuario", new { id = iduser });
                }

                return View();       

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
                        if(HttpContext.Session.GetInt32("id") == null)
                        {
                            HttpContext.Session.SetInt32("id", usuario.Id);
                            HttpContext.Session.SetString("nombre", usuario.Nombre);
                        }

                        return RedirectToAction("Index", "Home");
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
                List<SelectListItem> categoriaslistitemOrdenadas = categoriaslistitem.OrderBy(c => c.Text).ToList();
                return Json(categoriaslistitemOrdenadas);

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

        public async Task<IActionResult> PreguntasUsuarios(int? id)
        {
            if (id.HasValue)
            {
                var preguntasusuarios = await _preguntaservices.GetPreguntasUsuariosEspecificos(id.Value);
                ViewBag.Preguntas = preguntasusuarios;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }



    }
}
