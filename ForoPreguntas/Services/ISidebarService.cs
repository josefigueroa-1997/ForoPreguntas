using ForoPreguntas.Models;



namespace ForoPreguntas.Services
{
    public interface ISidebarService
    {
        List<Carrera> GetCarreras();
        List<Categoria> GetCategoriasGenerales(int idcarrera);
        List<Categoria> GetCategoriasUsuario(int idcarrera,int idusuario);
        Carrera GetCarreraUsuario(int idusuario);
    }
}
