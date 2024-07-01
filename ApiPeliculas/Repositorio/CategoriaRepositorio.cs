using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;

namespace ApiPeliculas.Repositorio;

public class CategoriaRepositorio : ICategoriaRepositorio {

    private readonly MiDbContext _bd;

    public CategoriaRepositorio(MiDbContext bd) {
        _bd = bd;
    }

    public bool ActualizarCategoria(Categoria categoria) {
        categoria.FechaCreacion = DateTime.Now;

        //Arreglar problema del PUT
        var categoriaExistente = _bd.Categorias.Find(categoria.Id);//Find busca en nuestro modelo
        if (categoriaExistente != null) {
            _bd.Entry(categoriaExistente).CurrentValues.SetValues(categoria); //CurrentValues son los valores, SetValues los inserta
        } else {
            _bd.Categorias.Update(categoria);
        }

        return Guardar();
    }
    public bool BorrarCategoria(Categoria categoria) {
        _bd.Categorias.Remove(categoria);
        return Guardar();
    }

    public bool CrearCategoria(Categoria categoria) {
        categoria.FechaCreacion = DateTime.Now;
        _bd.Categorias.Add(categoria);
        return Guardar();
    }

    public bool ExisteCategoria(string nombre) {
        bool existe = _bd.Categorias.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
        return existe;
    }

    public bool ExisteCategoria(int id) {
        bool existe = _bd.Categorias.Any(c => c.Id == id);
        return existe;
    }

    public Categoria GetCategoria(int categoriaid) {
        return _bd.Categorias.FirstOrDefault(c => c.Id == categoriaid);
    }

    public ICollection<Categoria> GetCategorias() {
        return _bd.Categorias.OrderBy(c => c.Nombre).ToList();
    }

    public bool Guardar() {
        return _bd.SaveChanges() >= 0 ? true : false;

    }
}
