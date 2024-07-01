using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.Dtos;

public class UsuarioLoginDto {

    [Required(ErrorMessage = "El Usuario es obligatorio")]
    public string NombreUsuario { get; set; }

    [Required(ErrorMessage = "El Password es obligatorio")]
    public string Password { get; set; }
    
   
   
}
