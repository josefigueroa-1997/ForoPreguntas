using ForoPreguntas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Dynamic;
using System.Diagnostics;
namespace ForoPreguntas.Services
{
    public class RespuestaService : IRespuestaService
    {
        private readonly FOROPREGUNTASContext _context;

        public RespuestaService(FOROPREGUNTASContext context)
        {
            _context = context;
        }

        public async Task<List<RespuestaPregunta>> GetRespuestasPregunta(int idpregunta)
        {
            try
            {
                List<RespuestaPregunta> respuestaPreguntas = new List<RespuestaPregunta>();
                respuestaPreguntas = await _context.RespuestaPreguntas.Where(rp => rp.ID_PREGUNTA == idpregunta).Include(r => r.Respuesta)
                    .OrderBy(rp => rp.Id).ToListAsync();

                return respuestaPreguntas;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<RespuestaPregunta>();
            }


        }
        

    }
}
