using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PeliculasController : ControllerBase {


    private readonly IPeliculaRepositorio _pelRepo;
    private readonly IMapper _mapper;

    public PeliculasController(IPeliculaRepositorio pelRepo, IMapper mapper) {
        _pelRepo = pelRepo;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetPeliculas() {
        var listaPeliculas = _pelRepo.GetPeliculas();
        var listaPeliculasDto = new List<PeliculaDto>();

        foreach (var pelicula in listaPeliculas) {
            listaPeliculasDto.Add(_mapper.Map<PeliculaDto>(pelicula));
        }

        return Ok(listaPeliculasDto);
    }


    [HttpGet("{peliculaId:int}", Name = "GetPelicula")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPelicula(int peliculaId) {
        var pelicula = _pelRepo.GetPelicula(peliculaId);
        if (pelicula == null) { return NotFound(); }
        var peliculaDto = _mapper.Map<PeliculaDto>(pelicula);
        return Ok(peliculaDto);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(PeliculaDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CrearPelicula([FromBody] CrearPeliculaDto crearPeliculaDto) {

        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        if (crearPeliculaDto == null) {
            return BadRequest();
        }
        if (_pelRepo.ExistePelicula(crearPeliculaDto.Nombre)) {
            ModelState.AddModelError("", "La película ya existe.");
            return StatusCode(404, ModelState);
        }

        var pelicula = _mapper.Map<Pelicula>(crearPeliculaDto);

        if (!_pelRepo.CrearPelicula(pelicula)) {
            ModelState.AddModelError("", $"Algo salio mal creando el registro {pelicula.Nombre}");
            return StatusCode(404, ModelState);
        }

        return CreatedAtRoute("GetPelicula", new { peliculaId = pelicula.Id }, pelicula);
    }



    [HttpPatch("{peliculaId:int}", Name = "ActualizarPatchPelicula")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ActualizarPatchPelicula(int peliculaId, [FromBody] CategoriaDto peliculaDto) {

        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        if (peliculaDto == null || peliculaId != peliculaDto.Id) {
            return BadRequest(ModelState);
        }

        var pelicula = _mapper.Map<Pelicula>(peliculaDto);

        if (!_pelRepo.ActualizarPelicula(pelicula)) {
            ModelState.AddModelError("", $"Algo salio mal actualizando el registro {pelicula.Nombre}");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }


    [HttpPut("{peliculaId:int}", Name = "ActualizarPutPelicula")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ActualizarPutPelicula(int peliculaId, [FromBody] PeliculaDto peliculaDto) {

        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        if (peliculaDto == null || peliculaId != peliculaDto.Id) {
            return BadRequest(ModelState);
        }


        if (!_pelRepo.ExistePelicula(peliculaId)) { return NotFound($"No se encontro la categoría con ID {peliculaId}"); }

        var pelicula = _mapper.Map<Pelicula>(peliculaDto);

        if (!_pelRepo.ActualizarPelicula(pelicula)) {
            ModelState.AddModelError("", $"Algo salio mal actualizando el registro {pelicula.Nombre}");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }


    [HttpDelete("{peliculaId:int}", Name = "BorrarPelicula")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult BorrarPelicula(int peliculaId) {
        if (!_pelRepo.ExistePelicula(peliculaId)) { return NotFound($"No se encontro la categoría con ID {peliculaId}"); }

        var pelicula = _pelRepo.GetPelicula(peliculaId);

        if (!_pelRepo.BorrarPelicula(pelicula)) {
            ModelState.AddModelError("", $"Algo salio mal borrando el registro {pelicula.Nombre}");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }


    [HttpGet("GetPelicualsEnCategorias/{categoriaId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetPelicualsEnCategorias(int categoriaId) {

        var lsitaPeliculas = _pelRepo.GetPeliculasEnCategoria(categoriaId);

        if (lsitaPeliculas == null) {
            return NotFound();
        }

        var listaPelicualasDto = new List<PeliculaDto>();

        foreach (var pelicula in lsitaPeliculas) {
            listaPelicualasDto.Add(_mapper.Map<PeliculaDto>(pelicula));
        }

        return Ok(listaPelicualasDto);
    }



    [HttpGet("BuscarPeliculas/{nombre}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult BuscarPeliculas(string nombre) {

        var listaPeliculas = _pelRepo.BuscarPelicula(nombre);

        if (listaPeliculas == null) {
            return NotFound();
        }

        var listaPelicualasDto = new List<PeliculaDto>();

        foreach (var pelicula in listaPeliculas) {
            listaPelicualasDto.Add(_mapper.Map<PeliculaDto>(pelicula));
        }

        return Ok(listaPelicualasDto);
    }





}
