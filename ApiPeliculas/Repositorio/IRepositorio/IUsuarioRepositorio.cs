using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;

namespace ApiPeliculas.Repositorio.IRepositorio;

public interface IUsuarioRepositorio {
    ICollection<Usuario> GetUsuarios();
    Usuario GetUsuario(int usuarioid);
    bool IsUniqueUser(string usuario);
    Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginRespuestaDto);
    Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto);
}
