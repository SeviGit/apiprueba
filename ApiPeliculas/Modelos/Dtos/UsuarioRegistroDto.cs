using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.Dtos;

public class UsuarioRegistroDto {

    [Required(ErrorMessage = "El Usuario es obligatorio")]
    public string NombreUsuario { get; set; }
    
    [Required(ErrorMessage = "El Usuario es obligatorio")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El Password es obligatorio")]
    public string Password { get; set; }
    public string Role { get; set; }
    
}
