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

        public Carrera GetCarreraUsuario(int idusuario)
        {
            try
            {
                var carrerausuario = _context.Usucarrcats.Where(uc => uc.ID_USUARIO == idusuario).Select(uc => uc.CarreraCategoria!=null ? uc.CarreraCategoria.Carrera : null).FirstOrDefault();
                return carrerausuario;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return new Carrera();
            }
        }

        public List<Categoria> GetCategoriasGenerales(int idcarrera)
        {
            try
            {
                List<Categoria> categorias = _context.CarreraCategorias
                    .Where(c => c.ID_CARRERA == idcarrera && c.Categoria != null)
                    .Select(c => new Categoria
                    {
                        Id = c.ID_CATEGORIA,
                        Nombre = c.Categoria.Nombre,
                    })
                    .ToList();

                Debug.WriteLine($"Categorías generales recuperadas para carrera ({idcarrera}): {string.Join(", ", categorias)}");

                return categorias;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error en GetCategoriasGenerales: {e.Message}");
                return new List<Categoria>();
            }
        }

        public List<Categoria> GetCategoriasUsuario(int idcarrera, int idusuario)
        {
            try
            {
                List<Categoria> categorias = _context.Usucarrcats
                    .Where(uc => uc.ID_CARRERA == idcarrera && uc.ID_USUARIO == idusuario && uc.CarreraCategoria != null && uc.CarreraCategoria.Categoria != null)
                    .Select(uc => new Categoria
                    {
                        Id = uc.ID_CATEGORIA,
                        Nombre = uc.CarreraCategoria.Categoria.Nombre,
                    })
                    .ToList();

                Debug.WriteLine($"Categorías para usuario ({idusuario}) en carrera ({idcarrera}): {string.Join(", ", categorias)}");

                return categorias;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error en GetCategoriasUsuario: {e.Message}");
                return new List<Categoria>();
            }
        }

    }
}
