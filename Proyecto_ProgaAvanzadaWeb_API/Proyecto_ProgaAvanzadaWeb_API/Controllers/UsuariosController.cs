using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;
using Proyecto_ProgaAvanzadaWeb_API.Services;
using System.Security.Claims;

namespace Proyecto_ProgaAvanzadaWeb_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var result = await _usuarioService.ObtenerTodos();

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

        [HttpGet("{id}")]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            try
            {
                var result = await _usuarioService.ObtenerPorId(id);

                if (result.Success)
                    return Ok(result);

                return NotFound(result);
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

        [HttpGet("perfil")]
        [Authorize]
        public async Task<IActionResult> ObtenerPerfil()
        {
            try
            {
                var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(idUsuarioClaim) || !long.TryParse(idUsuarioClaim, out long idUsuario))
                {
                    return BadRequest(new ResponseDTO<object>
                    {
                        Success = false,
                        Message = "Token inválido o usuario no encontrado"
                    });
                }

                var result = await _usuarioService.ObtenerPorId(idUsuario);

                if (result.Success)
                    return Ok(result);

                return NotFound(result);
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

        [HttpPut("perfil")]
        [Authorize]
        public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarPerfilDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(idUsuarioClaim) || !long.TryParse(idUsuarioClaim, out long idUsuario))
                {
                    return BadRequest(new ResponseDTO<object>
                    {
                        Success = false,
                        Message = "Token inválido o usuario no encontrado"
                    });
                }

                var result = await _usuarioService.ActualizarPerfil(idUsuario, dto);

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

        [HttpPost("cambiar-contrasena")]
        [Authorize]
        public async Task<IActionResult> CambiarContrasena([FromBody] CambiarContrasenaDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(idUsuarioClaim) || !long.TryParse(idUsuarioClaim, out long idUsuario))
                {
                    return BadRequest(new ResponseDTO<object>
                    {
                        Success = false,
                        Message = "Token inválido o usuario no encontrado"
                    });
                }

                var result = await _usuarioService.CambiarContrasena(idUsuario, dto);

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

        [HttpPut("{id}/estado")]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> CambiarEstado(long id, [FromBody] CambiarEstadoUsuarioDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _usuarioService.CambiarEstado(id, dto.Estado);

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

        [HttpPut("{id}")]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> ActualizarUsuario(long id, [FromBody] ActualizarUsuarioCompletoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _usuarioService.ActualizarUsuarioCompleto(id, dto);

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

        [HttpGet("estadisticas")]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> ObtenerEstadisticas()
        {
            try
            {
                var result = await _usuarioService.ObtenerEstadisticas();

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