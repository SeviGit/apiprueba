using ApiPeliculas.Modelos;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Data;

public class MiDbContext: DbContext{

    public MiDbContext (DbContextOptions<MiDbContext> options) : base(options) { }

    //Agregar los modelos aquí

    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Pelicula> Peliculas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

}
