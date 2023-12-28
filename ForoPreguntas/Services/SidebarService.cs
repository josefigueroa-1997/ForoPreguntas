using ForoPreguntas.Models;
using System.Diagnostics;

namespace ForoPreguntas.Services
{
    public class SidebarService : ISidebarService
    {
        private readonly FOROPREGUNTASContext _context;
        public SidebarService(FOROPREGUNTASContext context)
        {
            _context = context;
        }
        public List<Carrera> GetCarreras()
        {
            try
            {
                List<Carrera> carreras = _context.Carreras.Select(c => new Carrera
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                }).ToList();
                return carreras;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<Carrera>();
            }
        }
        public List<string?> GetCategorias(int id)
        {
            try
            {
                List<string?> categorias = _context.CarreraCategorias.Where(c => c.ID_CARRERA == id).Select(c => c.Categoria != null ? c.Categoria.Nombre : null).ToList();
                return categorias;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<string?>();
            }
        }

    }
}
