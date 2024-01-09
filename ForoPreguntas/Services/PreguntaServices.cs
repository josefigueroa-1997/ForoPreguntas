using ForoPreguntas.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Dynamic;

namespace ForoPreguntas.Services
{
    public class PreguntaServices : IPreguntaServices
    {
        private readonly FOROPREGUNTASContext _context;
        private readonly SidebarService _sidebarService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PreguntaServices(FOROPREGUNTASContext context, SidebarService sidebarService,IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _sidebarService = sidebarService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<dynamic>> GetPreguntaUsuarios(int? idpregunta, int? idcategoria, string? titulo)
        {
            try
            {
                if (!string.IsNullOrEmpty(titulo))
                {
                    return await GetPreguntaBYTittle(titulo);
                }
                if (_httpContextAccessor.HttpContext.Session.GetInt32("id") != null)
                {
                    return idcategoria.HasValue ? await GetPreguntaCategoria(idcategoria.Value) : await GetPreguntaCarreraUsuario();
                }

                return idcategoria.HasValue ? await GetPreguntaCategoria(idcategoria.Value) : await GetAllQuestions();
                 

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<dynamic>();
            }
        }

        public async Task<List<dynamic>> GetAllQuestions()
        {
            try
            {
                var preguntaUsuarios = await _context.PreguntaUsuarios
                    .Include(pu => pu.Pregunta)
                    .Include(u => u.Usucarrcat).ThenInclude(u => u.Usuario)
                    .OrderBy(pu => pu.Id)
                    .ToListAsync();

                var preguntaUsuariosEnumerable = preguntaUsuarios
                    .GroupBy(pu => pu.ID_PREGUNTA)
                    .Select(group => group.First())
                    .Select(pu => new
                    {
                        Id = pu.Id,
                        Idpregunta = pu.Pregunta?.Id,
                        Titulo = pu.Pregunta?.TITULO,
                        Detallepregunta = pu.Pregunta?.DETALLE_PREGUNTA,
                        NombreUsuario = pu.Usucarrcat?.Usuario?.Nombre,
                        FechaPublicacion = pu.Pregunta?.FECHA_PREGUNTA,
                    });
                
                
                var dynamicList = preguntaUsuariosEnumerable
                    .Select(p => {
                        dynamic expando = new ExpandoObject();
                        expando.Id = p.Id;
                        expando.Idpregunta = p.Idpregunta;
                        expando.Titulo = p.Titulo;
                        expando.Detallepregunta = p.Detallepregunta;
                        expando.NombreUsuario = p.NombreUsuario;
                        expando.FechaPublicacion = p.FechaPublicacion;
                        return expando;
                    }).ToList();
                dynamicList.Reverse();
                return dynamicList;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<dynamic>();
            }
        }

        public async Task<List<dynamic>> GetPreguntaCarreraUsuario()
        {
            try
            {
                int? idusuario = _httpContextAccessor.HttpContext.Session.GetInt32("id");

                Carrera carrera = _sidebarService.GetCarreraUsuario(idusuario.Value);
                var preguntaUsuarios = await _context.PreguntaUsuarios.Where(p=>p.Usucarrcat.ID_CARRERA == carrera.Id)
                    .Include(pu => pu.Pregunta)
                    .Include(u => u.Usucarrcat).ThenInclude(u => u.Usuario)
                    .OrderBy(pu => pu.Id)
                    .ToListAsync();

                var preguntaUsuariosEnumerable = preguntaUsuarios
                    .GroupBy(pu => pu.ID_PREGUNTA)
                    .Select(group => group.First())
                    .Select(pu => new
                    {
                        Id = pu.Id,
                        Idpregunta = pu.Pregunta?.Id,
                        Titulo = pu.Pregunta?.TITULO,
                        Detallepregunta = pu.Pregunta?.DETALLE_PREGUNTA,
                        NombreUsuario = pu.Usucarrcat?.Usuario?.Nombre,
                        FechaPublicacion = pu.Pregunta?.FECHA_PREGUNTA,
                    });


                var dynamicList = preguntaUsuariosEnumerable
                    .Select(p => {
                        dynamic expando = new ExpandoObject();
                        expando.Id = p.Id;
                        expando.Idpregunta = p.Idpregunta;
                        expando.Titulo = p.Titulo;
                        expando.Detallepregunta = p.Detallepregunta;
                        expando.NombreUsuario = p.NombreUsuario;
                        expando.FechaPublicacion = p.FechaPublicacion;
                        return expando;
                    }).ToList();
                dynamicList.Reverse();
                return dynamicList;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<dynamic>();
            }
        }

        

        public async Task<List<dynamic>> GetPreguntasUsuariosEspecificos(int idusuario)
        {
            try
            {
                var preguntaUsuarios = await _context.PreguntaUsuarios.Where(u=>u.Usucarrcat.ID_USUARIO == idusuario)
                    .Include(pu => pu.Pregunta)
                    .Include(u => u.Usucarrcat).ThenInclude(u => u.Usuario)
                    .OrderBy(pu => pu.Id)
                    .ToListAsync();

                var preguntaUsuariosEnumerable = preguntaUsuarios
                    .GroupBy(pu => pu.ID_PREGUNTA)
                    .Select(group => group.First())
                    .Select(pu => new
                    {
                        Id = pu.Id,
                        Idpregunta = pu.Pregunta?.Id,
                        Titulo = pu.Pregunta?.TITULO,
                        Detallepregunta = pu.Pregunta?.DETALLE_PREGUNTA,
                        NombreUsuario = pu.Usucarrcat?.Usuario?.Nombre,
                        FechaPublicacion = pu.Pregunta?.FECHA_PREGUNTA,
                    });


                var dynamicList = preguntaUsuariosEnumerable
                    .Select(p => {
                        dynamic expando = new ExpandoObject();
                        expando.Id = p.Id;
                        expando.Idpregunta = p.Idpregunta;
                        expando.Titulo = p.Titulo;
                        expando.Detallepregunta = p.Detallepregunta;
                        expando.NombreUsuario = p.NombreUsuario;
                        expando.FechaPublicacion = p.FechaPublicacion;
                        return expando;
                    }).ToList();
                dynamicList.Reverse();
                return dynamicList;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<dynamic>();
            }
        }

        public async Task<List<dynamic>> GetPreguntaCategoria(int idcategoria)
        {
            try
            {
                var preguntaUsuarios = await _context.PreguntaUsuarios.Where(u => u.Usucarrcat.ID_CATEGORIA == idcategoria)
                    .Include(pu => pu.Pregunta)
                    .Include(u => u.Usucarrcat).ThenInclude(u => u.Usuario)
                    .OrderBy(pu => pu.Id)
                    .ToListAsync();

                var preguntaUsuariosEnumerable = preguntaUsuarios
                    .GroupBy(pu => pu.ID_PREGUNTA)
                    .Select(group => group.First())
                    .Select(pu => new
                    {
                        Id = pu.Id,
                        Idpregunta = pu.Pregunta?.Id,
                        Titulo = pu.Pregunta?.TITULO,
                        Detallepregunta = pu.Pregunta?.DETALLE_PREGUNTA,
                        NombreUsuario = pu.Usucarrcat?.Usuario?.Nombre,
                        FechaPublicacion = pu.Pregunta?.FECHA_PREGUNTA,
                    });


                var dynamicList = preguntaUsuariosEnumerable
                    .Select(p => {
                        dynamic expando = new ExpandoObject();
                        expando.Id = p.Id;
                        expando.Idpregunta = p.Idpregunta;
                        expando.Titulo = p.Titulo;
                        expando.Detallepregunta = p.Detallepregunta;
                        expando.NombreUsuario = p.NombreUsuario;
                        expando.FechaPublicacion = p.FechaPublicacion;
                        return expando;
                    }).ToList();
                dynamicList.Reverse();
                return dynamicList;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<dynamic>();
            }
        }   

        public async Task<List<dynamic>> GetPreguntaBYTittle(string titulo)
        {
            try
            {
                var preguntaUsuarios = await _context.PreguntaUsuarios.Where(pu => EF.Functions.Like(pu.Pregunta.TITULO,'%'+ titulo + '%'))
                    .Include(pu => pu.Pregunta)
                    .Include(u => u.Usucarrcat).ThenInclude(u => u.Usuario)
                    .OrderBy(pu => pu.Id)
                    .ToListAsync();

                var preguntaUsuariosEnumerable = preguntaUsuarios
                    .GroupBy(pu => pu.ID_PREGUNTA)
                    .Select(group => group.First())
                    .Select(pu => new
                    {
                        Id = pu.Id,
                        Idpregunta = pu.Pregunta?.Id,
                        Titulo = pu.Pregunta?.TITULO,
                        Detallepregunta = pu.Pregunta?.DETALLE_PREGUNTA,
                        NombreUsuario = pu.Usucarrcat?.Usuario?.Nombre,
                        FechaPublicacion = pu.Pregunta?.FECHA_PREGUNTA,
                    });


                var dynamicList = preguntaUsuariosEnumerable
                    .Select(p => {
                        dynamic expando = new ExpandoObject();
                        expando.Id = p.Id;
                        expando.Idpregunta = p.Idpregunta;
                        expando.Titulo = p.Titulo;
                        expando.Detallepregunta = p.Detallepregunta;
                        expando.NombreUsuario = p.NombreUsuario;
                        expando.FechaPublicacion = p.FechaPublicacion;
                        return expando;
                    }).ToList();
                dynamicList.Reverse();
                return dynamicList;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<dynamic>();
            }
        }

        public async Task<List<dynamic>> GetPreguntaByID(int idpregunta)
        {
            try
            {
                var preguntaUsuarios = await _context.PreguntaUsuarios.Where(pu => pu.Id == idpregunta)
                    .Include(pu => pu.Pregunta)
                    .Include(u => u.Usucarrcat).ThenInclude(u => u.Usuario)
                    .OrderBy(pu => pu.Id)
                    .ToListAsync();

                var preguntaUsuariosEnumerable = preguntaUsuarios
                    .GroupBy(pu => pu.ID_PREGUNTA)
                    .Select(group => group.First())
                    .Select(pu => new
                    {
                        Id = pu.Id,
                        Idpregunta = pu.Pregunta?.Id,
                        Titulo = pu.Pregunta?.TITULO,
                        Detallepregunta = pu.Pregunta?.DETALLE_PREGUNTA,
                        NombreUsuario = pu.Usucarrcat?.Usuario?.Nombre,
                        FechaPublicacion = pu.Pregunta?.FECHA_PREGUNTA,
                    });


                var dynamicList = preguntaUsuariosEnumerable
                    .Select(p => {
                        dynamic expando = new ExpandoObject();
                        expando.Id = p.Id;
                        expando.Idpregunta = p.Idpregunta;
                        expando.Titulo = p.Titulo;
                        expando.Detallepregunta = p.Detallepregunta;
                        expando.NombreUsuario = p.NombreUsuario;
                        expando.FechaPublicacion = p.FechaPublicacion;
                        return expando;
                    }).ToList();
                dynamicList.Reverse();
                return dynamicList;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<dynamic>();
            }
        }

        /*public List<dynamic> GetPreguntaCategoria(int idcategoria)
        {
            try
            {

                var preguntas = _context.PreguntaUsuarios.Where(p => p.Usucarrcat.ID_CATEGORIA  == idcategoria).
                Include(p => p.Pregunta).Include(p => p.Usucarrcat).ThenInclude(p => p.Usuario).OrderBy(p => p.Id).ToList().
                GroupBy(p => p.ID_PREGUNTA).Select(group => group.First()).
                Select(p => new
                {
                    Id = p.Id,
                    Idpregunta = p.Pregunta != null ? (int?)p.Pregunta.Id : null,
                    Titulo = p.Pregunta != null ? p.Pregunta.TITULO : null,
                    Detallepregunta = p.Pregunta != null ? p.Pregunta.DETALLE_PREGUNTA : null,
                    NombreUsuario = p.Usucarrcat != null && p.Usucarrcat.Usuario != null ? p.Usucarrcat.Usuario.Nombre : null,
                    FechaPublicacion = p.Pregunta != null ? p.Pregunta.FECHA_PREGUNTA : null,
                }).ToList<dynamic>();
                preguntas.Reverse();
                return preguntas;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<dynamic> { };
            }
        }


        private List<dynamic> GetPreguntaCarreraUsuario()
        {
            try
            {
                int? idusuario = _httpContextAccessor.HttpContext.Session.GetInt32("id");
                
                Carrera carrera = _sidebarService.GetCarreraUsuario(idusuario.Value);
               
                    var preguntas = _context.PreguntaUsuarios.Where(p => p.Usucarrcat.ID_CARRERA == carrera.Id).
                    Include(p => p.Pregunta).Include(p => p.Usucarrcat).ThenInclude(p => p.Usuario).OrderBy(p => p.Id).ToList().
                    GroupBy(p => p.ID_PREGUNTA).Select(group => group.First()).
                    Select(p => new
                    {
                        Id = p.Id,
                        Idpregunta = p.Pregunta != null ? (int?)p.Pregunta.Id : null,
                        Titulo = p.Pregunta != null ? p.Pregunta.TITULO : null,
                        Detallepregunta = p.Pregunta != null ? p.Pregunta.DETALLE_PREGUNTA : null,
                        NombreUsuario = p.Usucarrcat != null && p.Usucarrcat.Usuario != null ? p.Usucarrcat.Usuario.Nombre : null,
                        FechaPublicacion = p.Pregunta != null ? p.Pregunta.FECHA_PREGUNTA : null,
                    }).ToList<dynamic>();
                    preguntas.Reverse();
                    return preguntas;
               
                
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<dynamic> { };
            }


        }
        public List<dynamic> GetPreguntasUsuariosEspecificos(int idusuario)
        {
            try
            {
                
                var preguntas = _context.PreguntaUsuarios.Where(p => p.Usucarrcat.ID_USUARIO == idusuario).
                Include(p => p.Pregunta).Include(p => p.Usucarrcat).ThenInclude(p => p.Usuario).OrderBy(p => p.Id).ToList().
                GroupBy(p => p.ID_PREGUNTA).Select(group => group.First()).
                Select(p => new
                {
                    Id = p.Id,
                    Idpregunta = p.Pregunta != null ? (int?)p.Pregunta.Id : null,
                    Titulo = p.Pregunta != null ? p.Pregunta.TITULO : null,
                    Detallepregunta = p.Pregunta != null ? p.Pregunta.DETALLE_PREGUNTA : null,
                    NombreUsuario = p.Usucarrcat != null && p.Usucarrcat.Usuario != null ? p.Usucarrcat.Usuario.Nombre : null,
                    FechaPublicacion = p.Pregunta != null ? p.Pregunta.FECHA_PREGUNTA : null,
                }).ToList<dynamic>();
                preguntas.Reverse();
                return preguntas;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<dynamic> { };
            }
        }*/
    }
}
