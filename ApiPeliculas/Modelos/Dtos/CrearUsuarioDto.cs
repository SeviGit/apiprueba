using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.Dtos;

public class CrearUsuarioDto {
    [Required(ErrorMessage ="El nombre es obligatorio")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El Usuario es obligatorio")]
    public string NombreUsuario { get; set; }

    [Required(ErrorMessage = "El Password es obligatorio")]
    public string Password { get; set; }
   
}
