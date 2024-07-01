using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.IdentityModel.Tokens;
using Sofitec.Librerias.Criptografia;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiPeliculas.Repositorio;

public class UsuarioRepositorio(MiDbContext _bd, IConfiguration config) : IUsuarioRepositorio {

    private string claveSecreta = config.GetValue<string>("apiSetting:Secreta");

    public ICollection<Usuario> GetUsuarios() => _bd.Usuarios.ToList().OrderBy(c => c.NombreUsuario).ToList();


    public Usuario GetUsuario(int usuarioid) => _bd.Usuarios.Where(u => u.Id == usuarioid).FirstOrDefault();

    public bool IsUniqueUser(string nombreusuario) {
        var usuario = _bd.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreusuario);
        if (usuario == null) return false; return true;

    }

    public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto) {
        var passwordEncristado = Criptografia.CrearHashMD5(usuarioLoginDto.Password);

        var usuario = _bd.Usuarios.FirstOrDefault(
            u => u.NombreUsuario.ToLower() == usuarioLoginDto.NombreUsuario.ToLower() &&
            u.Password == passwordEncristado
            );

        //Validamos si el usuario no existe con la combinacion de usuario y contraseña correcta
        if (usuario == null) {
            return new UsuarioLoginRespuestaDto {
                Token = "",
                Usuario = null
            };
        }

        //Aquí existe el usuario por lo que podemos procesar el login
        var manejadoToken = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(claveSecreta);

        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(
                new Claim[] {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario.ToString()),
                new Claim(ClaimTypes.Role,usuario.Role.ToString())
                }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };


        var token = manejadoToken.CreateToken(tokenDescriptor);

        UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new() {
            Token = manejadoToken.WriteToken(token),
            Usuario = usuario,
            Role = usuario.Role,
        };

        return usuarioLoginRespuestaDto;
    }


    public async Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto) {
        var passwordEncristado = Criptografia.CrearHashMD5(usuarioRegistroDto.Password);

        var usuario = new Usuario {
            NombreUsuario = usuarioRegistroDto.NombreUsuario,
            Nombre = usuarioRegistroDto.Nombre,
            Password = passwordEncristado,
            Role = usuarioRegistroDto.Role,
        };

        _bd.Usuarios.Add(usuario);
        await _bd.SaveChangesAsync();

        usuario.Password = passwordEncristado;
        return usuario;
    }
}


//Chat GPT
//public string GenerateToken() {
//    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
//    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//    var claims = new[]
//    {
//            new Claim(JwtRegisteredClaimNames.Sub, "usuario"),
//            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//        };

//    var token = new JwtSecurityToken(
//        issuer: _configuration["Jwt:Issuer"],
//        audience: _configuration["Jwt:Audience"],
//        claims: claims,
//        expires: DateTime.Now.AddMinutes(30),
//        signingCredentials: credentials);

//    return new JwtSecurityTokenHandler().WriteToken(token);
//}
//}