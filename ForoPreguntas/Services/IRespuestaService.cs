using ForoPreguntas.Models;

namespace ForoPreguntas.Services
{
    public interface IRespuestaService
    {
        Task<List<RespuestaPregunta>> GetRespuestasPregunta(int idpregunta);
        public bool AgregarRespuesta(string detalle,int idpregunta,IFormFile imagen);
        public void AgregarRespuestaPregunta(int idpregunta,int idrespuesta);
        public bool AgregarCalificacionRespuesta(int idrespuesta,int calificacion);
        public RespuestaPregunta GetRespuestaById(int idrespuesta);
    }
}
