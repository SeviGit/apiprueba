using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiPeliculas.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersionNeutral] // Indiferente para versiones, aparece en todas
    public class UsuariosController(IMapper mapper, IUsuarioRepositorio usuRepo) : ControllerBase {

        private readonly IMapper _mapper = mapper;
        private readonly IUsuarioRepositorio _usuRepo = usuRepo;
        protected RespuestasApi _respuestaApi = new();

        [Authorize(Roles = "adm")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsuarios() {
            var usuarios = _usuRepo.GetUsuarios();
            var usuariosDto = new List<UsuarioDto>();

            foreach (var usuario in usuarios) {
                usuariosDto.Add(_mapper.Map<UsuarioDto>(usuario));
            }

            return Ok(usuariosDto);
        }

        [Authorize(Roles = "adm")]
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


        [AllowAnonymous]
        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto usuarioRegistroDto) {

            bool isUniqueUser = _usuRepo.IsUniqueUser(usuarioRegistroDto.NombreUsuario);
            if (isUniqueUser) {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("El nombre del usuario ya existe");
                return BadRequest(_respuestaApi);
            }

            var usuario = await _usuRepo.Registro(usuarioRegistroDto);

            if (usuario == null) {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("Error en el registro");
                return BadRequest(_respuestaApi);
            }

            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.IsSuccess = true;
            _respuestaApi.Result = usuario;
            return Ok(_respuestaApi);

        }



        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(UsuarioLoginDto usuarioLoginRespuestaDto) {

            var respuestaLogin = await _usuRepo.Login(usuarioLoginRespuestaDto);

            if (respuestaLogin.Usuario == null || string.IsNullOrEmpty(respuestaLogin.Token)) {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("El nombre de usuario o password son incorrectos");
                return BadRequest(_respuestaApi);
            }

            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.IsSuccess = true;
            _respuestaApi.Result = respuestaLogin;


            return Ok(_respuestaApi);
        }

    }
}
