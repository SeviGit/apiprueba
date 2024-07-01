using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Repositorio;

public class PeliculaRepositorio(MiDbContext _bd) : IPeliculaRepositorio {


    public bool ActualizarPelicula(Pelicula pelicula) {
        pelicula.FechaCreacion = DateTime.Now;

        //Arreglar problema del PUT
        var PeliculaExistente = _bd.Peliculas.Find(pelicula.Id);//Find busca en nuestro modelo
        if (PeliculaExistente != null) {
            _bd.Entry(PeliculaExistente).CurrentValues.SetValues(pelicula); //CurrentValues son los valores, SetValues los inserta
        } else {
            _bd.Peliculas.Update(pelicula);
        }

        return Guardar();
    }
    public ICollection<Pelicula> GetPeliculasEnCategoria(int catId) => _bd.Peliculas.Include(ca => ca.Categoria).Where(p => p.categoriaId == catId).ToList();
    

    public IEnumerable<Pelicula> BuscarPelicula(string nombre) {
        IQueryable<Pelicula> query = _bd.Peliculas;
        if (!string.IsNullOrEmpty(nombre)) {
            query = query.Where(x => x.Nombre.Contains(nombre) || x.Descripcion.Contains(nombre));
        }
        return query.ToList();
    }

    public bool BorrarPelicula(Pelicula pelicula) {
        _bd.Peliculas.Remove(pelicula);
        return Guardar();
    }

    public bool CrearPelicula(Pelicula pelicula) {
        pelicula.FechaCreacion = DateTime.Now;
        _bd.Peliculas.Add(pelicula);
        return Guardar();
    }

    public bool ExistePelicula(string nombre) {
        bool existe = _bd.Peliculas.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
        return existe;
    }

    public bool ExistePelicula(int id) {
        bool existe = _bd.Peliculas.Any(c => c.Id == id);
        return existe;
    }

    public Pelicula GetPelicula(int peliculaid) {
        return _bd.Peliculas.FirstOrDefault(c => c.Id == peliculaid);
    }

    public ICollection<Pelicula> GetPeliculas() {
        return _bd.Peliculas.OrderBy(c => c.Nombre).ToList();
    }

    public bool Guardar() {
        return _bd.SaveChanges() >= 0 ? true : false;

    }
}
