using ForoPreguntas.Models;



namespace ForoPreguntas.Services
{
    public interface ISidebarService
    {
        List<Carrera> GetCarreras();
        List<string?> GetCategorias(int id);
    }
}
