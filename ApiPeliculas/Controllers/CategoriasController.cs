using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers;
//[Route("api/[controller]")] -- Una opción
[Route("api/categorias")] // otra opción
[ApiController]
public class CategoriasController : ControllerBase {

    private readonly ICategoriaRepositorio _ctRepo;
    private readonly IMapper _mapper;

    public CategoriasController(ICategoriaRepositorio ctRepo, IMapper mapper) {
        _ctRepo = ctRepo;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetCategorias() {
        var listaCategorias = _ctRepo.GetCategorias();
        var listaCategoriasDto = new List<CategoriaDto>();

        foreach (var categoria in listaCategorias) {
            listaCategoriasDto.Add(_mapper.Map<CategoriaDto>(categoria));
        }

        return Ok(listaCategoriasDto);

    }

    [HttpGet("{categoriaId:int}", Name = "GetCategoria")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetCategoria(int categoriaId) {
        var categoria = _ctRepo.GetCategoria(categoriaId);
        if (categoria == null) { return NotFound(); }
        var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
        return Ok(categoriaDto);
    }



    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult CrearCategoria([FromBody] CrearCategoriaDto crearCategoriaDto) {
        
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        if (crearCategoriaDto == null) {
            return BadRequest();
        }
        if (_ctRepo.ExisteCategoria(crearCategoriaDto.Nombre)) {
            ModelState.AddModelError("", "La categoria ya existe.");
            return StatusCode(404, ModelState);
        }

        var categoria = _mapper.Map<Categoria>(crearCategoriaDto);

        if (!_ctRepo.CrearCategoria(categoria)) {
            ModelState.AddModelError("", $"Algo salio mal creando el registro {categoria.Nombre}");
            return StatusCode(404, ModelState);
        }

        return CreatedAtRoute("GetCategoria", new {categoriaId = categoria.Id},categoria);
    }



    [HttpPatch("{categoriaId:int}", Name = "ActualizarPatchCategoria")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ActualizarPatchCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto) {

        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        if (categoriaDto == null || categoriaId != categoriaDto.Id) {
            return BadRequest(ModelState);
        }

        var categoria = _mapper.Map<Categoria>(categoriaDto);

        if (!_ctRepo.ActualizarCategoria(categoria)) {
            ModelState.AddModelError("", $"Algo salio mal actualizando el registro {categoria.Nombre}");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }


    [HttpPut("{categoriaId:int}", Name = "ActualizarPutCategoria")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ActualizarPutCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto) {

        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        if (categoriaDto == null || categoriaId != categoriaDto.Id) {
            return BadRequest(ModelState);
        }

        //var categoriaExiste = GetCategoria(categoriaId);
        if (!_ctRepo.ExisteCategoria(categoriaId)) { return NotFound($"No se encontro la categoría con ID {categoriaId}"); } 

        var categoria = _mapper.Map<Categoria>(categoriaDto);

        if (!_ctRepo.ActualizarCategoria(categoria)) {
            ModelState.AddModelError("", $"Algo salio mal actualizando el registro {categoria.Nombre}");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }


    [HttpDelete("{categoriaId:int}", Name = "BorrarCategoria")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult BorrarCategoria(int categoriaId) {      
        if (!_ctRepo.ExisteCategoria(categoriaId)) { return NotFound($"No se encontro la categoría con ID {categoriaId}"); }

        var categoria = _ctRepo.GetCategoria(categoriaId);

        if (!_ctRepo.BorrarCategoria(categoria)) {
            ModelState.AddModelError("", $"Algo salio mal borrando el registro {categoria.Nombre}");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
}
