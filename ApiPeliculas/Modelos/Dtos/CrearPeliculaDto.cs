namespace ApiPeliculas.Modelos.Dtos;

public class CrearPeliculaDto {
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int Duracion { get; set; }
    public string RutaImagen { get; set; }
    public enum TipoClasificacionC { Siete, Trece, Dieciseis, Dieciocho }
    public TipoClasificacionC Clasificacion { get; set; }
    public int CategoriaId { get; set; }

}
