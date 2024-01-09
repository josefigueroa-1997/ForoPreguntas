namespace ForoPreguntas.Services
{
    public interface IPreguntaServices
    {
        Task<List<dynamic>> GetPreguntaUsuarios(int? idpregunta, int? idcategoria,string? titulo);
        Task<List<dynamic>> GetAllQuestions();
        Task<List<dynamic>> GetPreguntaCarreraUsuario();
        Task<List<dynamic>> GetPreguntasUsuariosEspecificos(int idusuario);
        Task<List<dynamic>> GetPreguntaCategoria(int idcategoria);
        Task<List<dynamic>> GetPreguntaBYTittle(string titulo);
        Task<List<dynamic>> GetPreguntaByID(int idpregunta);
    }
}
