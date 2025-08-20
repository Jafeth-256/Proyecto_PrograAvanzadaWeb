using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;
using Proyecto_ProgaAvanzadaWeb_API.Services;
using System.Security.Claims;

namespace Proyecto_ProgaAvanzadaWeb_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilController : ControllerBase
    {
        private readonly IPerfilService _perfilService;

        public PerfilController(IPerfilService perfilService)
        {
            _perfilService = perfilService;
        }

        /// <summary>
        /// Obtiene el perfil completo del usuario autenticado
        /// </summary>
        [HttpGet]
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

                var result = await _perfilService.ObtenerPerfilCompleto(idUsuario);

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

        /// <summary>
        /// Actualiza la información básica del perfil del usuario autenticado
        /// </summary>
        [HttpPut("basico")]
        [Authorize]
        public async Task<IActionResult> ActualizarPerfilBasico([FromBody] ActualizarPerfilBasicoDTO dto)
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

                var result = await _perfilService.ActualizarPerfilBasico(idUsuario, dto);

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

        /// <summary>
        /// Actualiza la información adicional del perfil del usuario autenticado
        /// </summary>
        [HttpPut("adicional")]
        [Authorize]
        public async Task<IActionResult> ActualizarInformacionAdicional([FromBody] ActualizarInformacionAdicionalDTO dto)
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

                var result = await _perfilService.ActualizarInformacionAdicional(idUsuario, dto);

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

        /// <summary>
        /// Cambia la contraseña del usuario autenticado
        /// </summary>
        [HttpPost("cambiar-contrasena")]
        [Authorize]
        public async Task<IActionResult> CambiarContrasena([FromBody] CambiarContrasenaPerfilDTO dto)
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

                var result = await _perfilService.CambiarContrasena(idUsuario, dto);

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

        /// <summary>
        /// Sube una foto de perfil para el usuario autenticado
        /// </summary>
        [HttpPost("subir-foto")]
        [Authorize]
        public async Task<IActionResult> SubirFotoPerfil([FromForm] SubirFotoPerfilDTO dto)
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

                var result = await _perfilService.SubirFotoPerfil(idUsuario, dto.Foto);

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