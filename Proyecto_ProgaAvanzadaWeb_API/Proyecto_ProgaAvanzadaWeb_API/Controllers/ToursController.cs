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
    public class ToursController : ControllerBase
    {
        private readonly DataContext _context;

        public ToursController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los tours disponibles - Acceso público
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                using var connection = _context.CreateConnection();
                var tours = await connection.QueryAsync<Tour>(
                    "ConsultarTours",
                    commandType: CommandType.StoredProcedure
                );

                var toursDto = tours.Select(t => new TourDTO
                {
                    IdTour = t.IdTour,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    Destino = t.Destino,
                    Precio = t.Precio,
                    FechaInicio = t.FechaInicio,
                    FechaFin = t.FechaFin,
                    CantidadPersonas = t.CantidadPersonas,
                    NombreCreador = t.NombreCreador
                });

                return Ok(new ResponseDTO<IEnumerable<TourDTO>>
                {
                    Success = true,
                    Message = "Tours obtenidos exitosamente",
                    Data = toursDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<object>
                {
                    Success = false,
                    Message = $"Error al obtener tours: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Obtiene un tour por ID - Acceso público
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var tour = await connection.QueryFirstOrDefaultAsync<Tour>(
                    "ConsultarTourPorId",
                    new { IdTour = id },
                    commandType: CommandType.StoredProcedure
                );

                if (tour == null)
                {
                    return NotFound(new ResponseDTO<object>
                    {
                        Success = false,
                        Message = "Tour no encontrado"
                    });
                }

                var tourDto = new TourDTO
                {
                    IdTour = tour.IdTour,
                    Nombre = tour.Nombre,
                    Descripcion = tour.Descripcion,
                    Destino = tour.Destino,
                    Precio = tour.Precio,
                    FechaInicio = tour.FechaInicio,
                    FechaFin = tour.FechaFin,
                    CantidadPersonas = tour.CantidadPersonas,
                    NombreCreador = tour.NombreCreador
                };

                return Ok(new ResponseDTO<TourDTO>
                {
                    Success = true,
                    Message = "Tour obtenido exitosamente",
                    Data = tourDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<object>
                {
                    Success = false,
                    Message = $"Error al obtener tour: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Crea un nuevo tour - Solo administradores
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> Crear([FromBody] CrearTourDTO dto)
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
                    "RegistrarTour",
                    new
                    {
                        Nombre = dto.Nombre,
                        Descripcion = dto.Descripcion,
                        Destino = dto.Destino,
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
                    Message = $"Error al crear tour: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Actualiza un tour existente - Solo administradores
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] CrearTourDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                using var connection = _context.CreateConnection();
                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "ActualizarTour",
                    new
                    {
                        IdTour = id,
                        Nombre = dto.Nombre,
                        Descripcion = dto.Descripcion,
                        Destino = dto.Destino,
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
                    Message = $"Error al actualizar tour: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Elimina un tour (eliminación lógica) - Solo administradores
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> Eliminar(long id)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "EliminarTour",
                    new { IdTour = id },
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
                    Message = $"Error al eliminar tour: {ex.Message}"
                });
            }
        }
    }
}