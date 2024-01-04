using ForoPreguntas.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ForoPreguntas.Services
{
    public class PreguntaServices
    {
        private readonly FOROPREGUNTASContext _context;

        public PreguntaServices(FOROPREGUNTASContext context)
        {
            _context = context;
        }
        public List<dynamic> GetPreguntaUsuarios(int? idpregunta, int? idcarrera, int? idcategoria, int? idusuario)
        {
            try
            {
                var preguntaUsuarios = _context.PreguntaUsuarios
                    .Include(pu => pu.Pregunta).Include(u=>u.Usucarrcat).ThenInclude(u=>u.Usuario)
                    .OrderByDescending(pu => pu.Id).GroupBy(pu=>pu.ID_PREGUNTA).Select(group=>group.First())
                    .ToList()
                    .Select(pu => new
                    {
                        Id = pu.Id,
                        Titulo = pu.Pregunta?.TITULO,
                        NombreUsuario = pu.Usucarrcat?.Usuario?.Nombre,
                        
                    })
                    .ToList<dynamic>();
                Debug.WriteLine($"El Tamaño de preguntausuario es:{preguntaUsuarios.Count}");
                foreach(var preguntas in preguntaUsuarios)
                {
                    Debug.WriteLine($"Titulo de Pregunta: {preguntas.NombreUsuario}");
                }
                return preguntaUsuarios;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<dynamic>();
            }
        }




    }
}
