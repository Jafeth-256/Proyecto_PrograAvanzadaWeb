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
    public class EventosUniversitariosController : ControllerBase
    {
        private readonly DataContext _context;

        public EventosUniversitariosController(DataContext context)
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
                var eventos = await connection.QueryAsync<EventoUniversitario>(
                    "ConsultarEventosUniversitarios",
                    commandType: CommandType.StoredProcedure
                );

                var eventosDto = eventos.Select(e => new EventoUniversitarioDTO
                {
                    IdEventoUniversitario = e.IdEventoUniversitario,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion,
                    Ubicacion = e.Ubicacion,
                    Universidad = e.Universidad,
                    Precio = e.Precio,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    CantidadPersonas = e.CantidadPersonas,
                    NombreCreador = e.NombreCreador
                });

                return Ok(new ResponseDTO<IEnumerable<EventoUniversitarioDTO>>
                {
                    Success = true,
                    Message = "Eventos universitarios obtenidos exitosamente",
                    Data = eventosDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<object>
                {
                    Success = false,
                    Message = $"Error al obtener eventos universitarios: {ex.Message}"
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
                var evento = await connection.QueryFirstOrDefaultAsync<EventoUniversitario>(
                    "ConsultarEventoUniversitarioPorId",
                    new { IdEventoUniversitario = id },
                    commandType: CommandType.StoredProcedure
                );

                if (evento == null)
                {
                    return NotFound(new ResponseDTO<object>
                    {
                        Success = false,
                        Message = "Evento universitario no encontrado"
                    });
                }

                var eventoDto = new EventoUniversitarioDTO
                {
                    IdEventoUniversitario = evento.IdEventoUniversitario,
                    Nombre = evento.Nombre,
                    Descripcion = evento.Descripcion,
                    Ubicacion = evento.Ubicacion,
                    Universidad = evento.Universidad,
                    Precio = evento.Precio,
                    FechaInicio = evento.FechaInicio,
                    FechaFin = evento.FechaFin,
                    CantidadPersonas = evento.CantidadPersonas,
                    NombreCreador = evento.NombreCreador
                };

                return Ok(new ResponseDTO<EventoUniversitarioDTO>
                {
                    Success = true,
                    Message = "Evento universitario obtenido exitosamente",
                    Data = eventoDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<object>
                {
                    Success = false,
                    Message = $"Error al obtener evento universitario: {ex.Message}"
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> Crear([FromBody] CrearEventoUniversitarioDTO dto)
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
                    "RegistrarEventoUniversitario",
                    new
                    {
                        Nombre = dto.Nombre,
                        Descripcion = dto.Descripcion,
                        Ubicacion = dto.Ubicacion,
                        Universidad = dto.Universidad,
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
                    Message = $"Error al crear evento universitario: {ex.Message}"
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] CrearEventoUniversitarioDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                using var connection = _context.CreateConnection();
                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "ActualizarEventoUniversitario",
                    new
                    {
                        IdEventoUniversitario = id,
                        Nombre = dto.Nombre,
                        Descripcion = dto.Descripcion,
                        Ubicacion = dto.Ubicacion,
                        Universidad = dto.Universidad,
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
                    Message = $"Error al actualizar evento universitario: {ex.Message}"
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
                    "EliminarEventoUniversitario",
                    new { IdEventoUniversitario = id },
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
                    Message = $"Error al eliminar evento universitario: {ex.Message}"
                });
            }
        }
    }
}