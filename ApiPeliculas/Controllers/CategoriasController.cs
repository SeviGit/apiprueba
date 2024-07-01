using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers;
//[Route("api/[controller]")] -- Una opción
//[Authorize(Roles = "Admin")]
//[ResponseCache(Duration = 20)]
[Route("api/categorias")] // otra opción
[ApiController]
//*** [EnableCors("PoliticaCors")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class CategoriasController : ControllerBase {

    private readonly ICategoriaRepositorio _ctRepo;
    private readonly IMapper _mapper;

    public CategoriasController(ICategoriaRepositorio ctRepo, IMapper mapper) {
        _ctRepo = ctRepo;
        _mapper = mapper;
    }


    [AllowAnonymous]
    [HttpGet]
    [MapToApiVersion(1.0)]
    [ResponseCache(CacheProfileName = "PorDefecto30Segundos")] 
    //[ResponseCache(Duration = 20)] 
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    //*** [EnableCors("PoliticaCors")] //Aplica la política CORS(diferentes dominios) a este método. esto si fuera por metodos, ahora esta de manera global
    public IActionResult GetCategorias() {
        var listaCategorias = _ctRepo.GetCategorias();
        var listaCategoriasDto = new List<CategoriaDto>();

        foreach (var categoria in listaCategorias) {
            listaCategoriasDto.Add(_mapper.Map<CategoriaDto>(categoria));
        }

        return Ok(listaCategoriasDto);

    }

    [AllowAnonymous]
    [HttpGet(Name = "Get")]
    [MapToApiVersion(1.0)]
    public IActionResult Get() {
        return Ok(true);
    }

    [HttpGet("{categoriaId:int}", Name = "GetCategoria")]
    //[ResponseCache(Duration = 40)]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore =true)] // la respuesta no debe almacenarse
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


    [Authorize(Roles = "adm")]
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


    [Authorize(Roles = "adm")]
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

    [Authorize(Roles = "adm")]
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

    [Authorize(Roles = "adm")]
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
