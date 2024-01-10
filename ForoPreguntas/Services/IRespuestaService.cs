using ForoPreguntas.Models;

namespace ForoPreguntas.Services
{
    public interface IRespuestaService
    {
        Task<List<RespuestaPregunta>> GetRespuestasPregunta(int idpregunta);
    }
}
