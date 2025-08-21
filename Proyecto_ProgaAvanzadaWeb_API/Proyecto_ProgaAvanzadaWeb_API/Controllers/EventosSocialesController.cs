using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;
using Proyecto_ProgaAvanzadaWeb_API.Models.Entities;
using Proyecto_PrograAvanzadaWeb_API.Data;
using System.Data;
using System.Security.Claims;

namespace Proyecto_ProgaAvanzadaWeb_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventosSocialesController : ControllerBase
    {
        private readonly DataContext _context;

        public EventosSocialesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                using var connection = _context.CreateConnection();
                var eventos = await connection.QueryAsync<EventoSocial>(
                    "ConsultarEventosSociales",
                    commandType: CommandType.StoredProcedure
                );

                var eventosDto = eventos.Select(e => new EventoSocialDTO
                {
                    IdEventoSocial = e.IdEventoSocial,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion,
                    Ubicacion = e.Ubicacion,
                    Precio = e.Precio,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    CantidadPersonas = e.CantidadPersonas,
                    NombreCreador = e.NombreCreador
                });

                return Ok(new ResponseDTO<IEnumerable<EventoSocialDTO>>
                {
                    Success = true,
                    Message = "Eventos sociales obtenidos exitosamente",
                    Data = eventosDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<object>
                {
                    Success = false,
                    Message = $"Error al obtener eventos sociales: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var evento = await connection.QueryFirstOrDefaultAsync<EventoSocial>(
                    "ConsultarEventoSocialPorId",
                    new { IdEventoSocial = id },
                    commandType: CommandType.StoredProcedure
                );

                if (evento == null)
                {
                    return NotFound(new ResponseDTO<object>
                    {
                        Success = false,
                        Message = "Evento social no encontrado"
                    });
                }

                var eventoDto = new EventoSocialDTO
                {
                    IdEventoSocial = evento.IdEventoSocial,
                    Nombre = evento.Nombre,
                    Descripcion = evento.Descripcion,
                    Ubicacion = evento.Ubicacion,
                    Precio = evento.Precio,
                    FechaInicio = evento.FechaInicio,
                    FechaFin = evento.FechaFin,
                    CantidadPersonas = evento.CantidadPersonas,
                    NombreCreador = evento.NombreCreador
                };

                return Ok(new ResponseDTO<EventoSocialDTO>
                {
                    Success = true,
                    Message = "Evento social obtenido exitosamente",
                    Data = eventoDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<object>
                {
                    Success = false,
                    Message = $"Error al obtener evento social: {ex.Message}"
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> Crear([FromBody] CrearEventoSocialDTO dto)
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

                using var connection = _context.CreateConnection();
                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "RegistrarEventoSocial",
                    new
                    {
                        Nombre = dto.Nombre,
                        Descripcion = dto.Descripcion,
                        Ubicacion = dto.Ubicacion,
                        Precio = dto.Precio,
                        FechaInicio = dto.FechaInicio,
                        FechaFin = dto.FechaFin,
                        CantidadPersonas = dto.CantidadPersonas,
                        IdUsuarioCreador = idUsuario
                    },
                    commandType: CommandType.StoredProcedure
                );

                return Ok(new ResponseDTO<object>
                {
                    Success = resultado.Resultado > 0,
                    Message = resultado.Mensaje
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<object>
                {
                    Success = false,
                    Message = $"Error al crear evento social: {ex.Message}"
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] CrearEventoSocialDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                using var connection = _context.CreateConnection();
                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "ActualizarEventoSocial",
                    new
                    {
                        IdEventoSocial = id,
                        Nombre = dto.Nombre,
                        Descripcion = dto.Descripcion,
                        Ubicacion = dto.Ubicacion,
                        Precio = dto.Precio,
                        FechaInicio = dto.FechaInicio,
                        FechaFin = dto.FechaFin,
                        CantidadPersonas = dto.CantidadPersonas
                    },
                    commandType: CommandType.StoredProcedure
                );

                return Ok(new ResponseDTO<object>
                {
                    Success = resultado.Resultado > 0,
                    Message = resultado.Mensaje
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<object>
                {
                    Success = false,
                    Message = $"Error al actualizar evento social: {ex.Message}"
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> Eliminar(long id)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "EliminarEventoSocial",
                    new { IdEventoSocial = id },
                    commandType: CommandType.StoredProcedure
                );

                return Ok(new ResponseDTO<object>
                {
                    Success = resultado.Resultado > 0,
                    Message = resultado.Mensaje
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<object>
                {
                    Success = false,
                    Message = $"Error al eliminar evento social: {ex.Message}"
                });
            }
        }
    }
}