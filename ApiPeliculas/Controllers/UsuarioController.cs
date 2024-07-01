using ApiPeliculas.Data;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net;

namespace ApiPeliculas.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController (IMapper mapper, IUsuarioRepositorio usuRepo , RespuestasApi respuestaApi): ControllerBase {

        private readonly IMapper _mapper = mapper;
        private readonly IUsuarioRepositorio _usuRepo = usuRepo;
        protected  RespuestasApi _respuestaApi = respuestaApi;


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsuarios() {
            var usuarios = _usuRepo.GetUsuarios();
            var usuariosDto = new List<UsuarioDto>();

            foreach ( var usuario in usuarios) {
                usuariosDto.Add(_mapper.Map<UsuarioDto>(usuario));  
            }

            return Ok(usuariosDto);
        }


        [HttpGet("{usuarioId}", Name = "GetUsuario")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetUsuario(int usuarioId) {
            var usuario = _usuRepo.GetUsuario(usuarioId);
            if (usuario == null) {
                return NotFound();
            }

            return Ok(_mapper.Map<UsuarioDto>(usuario));
        }



        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto usuarioRegistroDto) {

            bool isUniqueUser = _usuRepo.IsUniqueUser(usuarioRegistroDto.NombreUsuario);
            if (!isUniqueUser) {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("El nombre del usuario ya existe");
                return BadRequest(_respuestaApi);
            }

            var usuario = _usuRepo.Registro(usuarioRegistroDto);

            if (usuario == null) {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("Error en el registro");
                return BadRequest(_respuestaApi);
            }
            
            return Ok(usuario);
        }

        public IActionResult IsUniqueUser(string usuario) {
            var isUniqueUser = _usuRepo.IsUniqueUser(usuario);
            return Ok(isUniqueUser);

        }
        //public IActionResult Login(UsuarioLoginDto usuarioLoginRespuestaDto) {

        //}

    }
}
