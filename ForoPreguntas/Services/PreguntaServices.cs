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

        public async Task<List<PreguntaUsuario>> GetPreguntaUsuarios(int? idpregunta, int? idcategoria, string? titulo)
        {
            try
            {
                if (idpregunta.HasValue)
                {
                    return await GetPreguntaByID(idpregunta.Value);
                }
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
                return new List<PreguntaUsuario>();
            }
        }

        public async Task<List<PreguntaUsuario>> GetAllQuestions()
        {
            try
            {
                List<PreguntaUsuario> preguntausuarios = new List<PreguntaUsuario>();

                preguntausuarios = await _context.PreguntaUsuarios
                    .Include(pu => pu.Pregunta)
                    .Include(u => u.Usucarrcat).ThenInclude(u => u.Usuario)
                    .OrderBy(pu => pu.Id).GroupBy(pu => pu.ID_PREGUNTA)
                    .Select(group => group.First())
                    .ToListAsync();
                preguntausuarios.Reverse();

                return preguntausuarios;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<PreguntaUsuario>();
            }
        }

        public async Task<List<PreguntaUsuario>> GetPreguntaCarreraUsuario()
        {
            try
            {
                List<PreguntaUsuario> preguntaUsuarios = new List<PreguntaUsuario>();
                int? idusuario = _httpContextAccessor.HttpContext.Session.GetInt32("id");

                Carrera carrera = _sidebarService.GetCarreraUsuario(idusuario.Value);
                preguntaUsuarios = await _context.PreguntaUsuarios.Where(p => p.Usucarrcat.ID_CARRERA == carrera.Id)
                    .Include(pu => pu.Pregunta)
                    .Include(u => u.Usucarrcat).ThenInclude(u => u.Usuario)
                    .OrderBy(pu => pu.Id).GroupBy(pu => pu.ID_PREGUNTA)
                     .Select(group => group.First())
                    .ToListAsync();
                preguntaUsuarios.Reverse();
                return preguntaUsuarios;
               
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<PreguntaUsuario>();
            }
        }

        

        public async Task<List<PreguntaUsuario>> GetPreguntasUsuariosEspecificos(int idusuario)
        {
            try
            {
                List<PreguntaUsuario> preguntaUsuarios = new List<PreguntaUsuario>();

                preguntaUsuarios = await _context.PreguntaUsuarios.Where(u => u.Usucarrcat.ID_USUARIO == idusuario)
                    .Include(pu => pu.Pregunta)
                    .Include(u => u.Usucarrcat).ThenInclude(u => u.Usuario)
                    .OrderBy(pu => pu.Id).GroupBy(pu => pu.ID_PREGUNTA)
                    .Select(group => group.First())
                    .ToListAsync();
                preguntaUsuarios.Reverse();
                return preguntaUsuarios;
               
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<PreguntaUsuario>();
            }
        }

        public async Task<List<PreguntaUsuario>> GetPreguntaCategoria(int idcategoria)
        {
            try
            {
                List<PreguntaUsuario> preguntaUsuarios = new List<PreguntaUsuario>();
                preguntaUsuarios = await _context.PreguntaUsuarios.Where(u => u.Usucarrcat.ID_CATEGORIA == idcategoria)
                    .Include(pu => pu.Pregunta)
                    .Include(u => u.Usucarrcat).ThenInclude(u => u.Usuario)
                    .OrderBy(pu => pu.Id).GroupBy(pu => pu.ID_PREGUNTA)
                    .Select(group => group.First())
                    .ToListAsync();
                preguntaUsuarios.Reverse();
                return preguntaUsuarios;
                
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<PreguntaUsuario>();
            }
        }   

        public async Task<List<PreguntaUsuario>> GetPreguntaBYTittle(string titulo)
        {
            try
            {
                List<PreguntaUsuario> preguntaUsuarios = new List<PreguntaUsuario>();
                preguntaUsuarios = await _context.PreguntaUsuarios.Where(pu => EF.Functions.Like(pu.Pregunta.TITULO, '%' + titulo + '%'))
                    .Include(pu => pu.Pregunta)
                    .Include(u => u.Usucarrcat).ThenInclude(u => u.Usuario)
                    .OrderBy(pu => pu.Id)
                    .ToListAsync();
                preguntaUsuarios.Reverse();
                return preguntaUsuarios;
                
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<PreguntaUsuario>();
            }
        }

        public async Task<List<PreguntaUsuario>> GetPreguntaByID(int idpregunta)
        {
            try
            {
                List<PreguntaUsuario> preguntaUsuarios = new List<PreguntaUsuario>();
                preguntaUsuarios = await _context.PreguntaUsuarios.Where(pu => pu.Id == idpregunta)
                    .Include(pu => pu.Pregunta).Include(ucc=>ucc.Usucarrcat).ThenInclude(cc=>cc.CarreraCategoria).ThenInclude(c=>c.Categoria)
                    .Include(u => u.Usucarrcat).ThenInclude(u => u.Usuario)
                    .OrderBy(pu => pu.Id)
                    .ToListAsync();
                return preguntaUsuarios;
               
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<PreguntaUsuario>();
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
