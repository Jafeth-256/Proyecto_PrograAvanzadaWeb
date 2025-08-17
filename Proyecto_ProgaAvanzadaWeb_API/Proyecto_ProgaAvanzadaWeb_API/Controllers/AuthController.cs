using Microsoft.AspNetCore.Mvc;
using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;
using Proyecto_ProgaAvanzadaWeb_API.Services;

namespace Proyecto_ProgaAvanzadaWeb_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Iniciar sesión
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.Login(loginDto);

                if (result.Success)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new LoginResponseDTO
                {
                    Success = false,
                    Message = $"Error interno del servidor: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Registrar nuevo usuario
        /// </summary>
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistroUsuarioDTO registroDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.Registrar(registroDto);

                if (result.Success)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<object>
                {
                    Success = false,
                    Message = $"Error interno del servidor: {ex.Message}"
                });
            }
        }
    }
}