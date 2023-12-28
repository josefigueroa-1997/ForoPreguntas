namespace ForoPreguntas.Models
{
    public partial class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public virtual ICollection<CarreraCategoria> CarreraCategorias { get; set; } = new List<CarreraCategoria>();

    }
}
