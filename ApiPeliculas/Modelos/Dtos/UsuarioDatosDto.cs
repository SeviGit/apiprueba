using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.Dtos;

public class UsuarioDatosDto {
    public string NombreUsuario { get; set; }
    public string Nombre { get; set; }
    public string Password { get; set; }
   
}
