using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;
using Proyecto_ProgaAvanzadaWeb_API.Services;
using System.Security.Claims;

namespace Proyecto_ProgaAvanzadaWeb_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasSocialesController : ControllerBase
    {
        private readonly IReservaSocialService _reservaSocialService;

        public ReservasSocialesController(IReservaSocialService reservaSocialService)
        {
            _reservaSocialService = reservaSocialService;
        }

        [HttpGet("eventos-disponibles")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerEventosSocialesDisponibles()
        {
            try
            {
                var result = await _reservaSocialService.ObtenerEventosSocialesDisponibles();

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

        [HttpGet("eventos-disponibles/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerEventoSocialDisponiblePorId(long id)
        {
            try
            {
                var result = await _reservaSocialService.ObtenerEventoSocialDisponiblePorId(id);

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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CrearReservaSocial([FromBody] CrearReservaSocialDTO dto)
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

                var result = await _reservaSocialService.CrearReservaSocial(idUsuario, dto);

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

        [HttpGet("mis-reservas")]
        [Authorize]
        public async Task<IActionResult> ObtenerMisReservasSociales()
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

                var result = await _reservaSocialService.ObtenerReservasSocialesUsuario(idUsuario);

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

        [HttpPost("{id}/cancelar")]
        [Authorize]
        public async Task<IActionResult> CancelarReservaSocial(long id)
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

                var result = await _reservaSocialService.CancelarReservaSocial(id, idUsuario);

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

        [HttpGet("admin/todas")]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var result = await _reservaSocialService.ObtenerTodasLasReservasSociales();

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

        [HttpPut("admin/{id}/estado")]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> ActualizarEstado(long id, [FromBody] string nuevoEstado)
        {
            try
            {
                var result = await _reservaSocialService.ActualizarEstadoReservaSocialAdmin(id, nuevoEstado);

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
