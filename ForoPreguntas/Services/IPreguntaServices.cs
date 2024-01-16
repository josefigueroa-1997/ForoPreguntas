using ForoPreguntas.Models;

namespace ForoPreguntas.Services
{
    public interface IPreguntaServices
    {
        Task<List<PreguntaUsuario>> GetPreguntaUsuarios(int? idpregunta, int? idcategoria,string? titulo);
        Task<List<PreguntaUsuario>> GetAllQuestions();
        Task<List<PreguntaUsuario>> GetPreguntaCarreraUsuario();
        Task<List<PreguntaUsuario>> GetPreguntasUsuariosEspecificos(int idusuario);
        Task<List<PreguntaUsuario>> GetPreguntaCategoria(int idcategoria);
        Task<List<PreguntaUsuario>> GetPreguntaBYTittle(string titulo);
        Task<List<PreguntaUsuario>> GetPreguntaByID(int idpregunta);
        
    }
}
