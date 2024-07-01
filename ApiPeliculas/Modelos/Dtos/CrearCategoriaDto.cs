using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.Dtos;

public class CrearCategoriaDto {

    //Esta validación es importante sino se crear vacía el nombre de categoría
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(60, ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; }
}
