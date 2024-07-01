using ApiPeliculas.Modelos;

namespace ApiPeliculas.Repositorio.IRepositorio;

public interface IPeliculaRepositorio {
    ICollection<Pelicula> GetPeliculas();
    ICollection<Pelicula> GetPeliculasEnCategoria(int catId);
    IEnumerable<Pelicula> BuscarPelicula(string nombre);
    Pelicula GetPelicula(int peliculaid);
    bool ExistePelicula(string nombre);
    bool ExistePelicula(int id);
    bool CrearPelicula(Pelicula pelicula);
    bool ActualizarPelicula(Pelicula pelicula);
    bool BorrarPelicula(Pelicula pelicula);
    bool Guardar();
}
