using Dapper;
using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;
using Proyecto_ProgaAvanzadaWeb_API.Models.Entities;
using Proyecto_PrograAvanzadaWeb_API.Data;
using System.Data;

namespace Proyecto_ProgaAvanzadaWeb_API.Services
{
    public class ReservaService : IReservaService
    {
        private readonly DataContext _context;

        public ReservaService(DataContext context)
        {
            _context = context;
        }

        public async Task<ResponseDTO<IEnumerable<TourDisponible>>> ObtenerToursDisponibles()
        {
            try
            {
                using var connection = _context.CreateConnection();
                var tours = await connection.QueryAsync<TourDisponible>(
                    "ConsultarToursDisponibles",
                    commandType: CommandType.StoredProcedure
                );

                return new ResponseDTO<IEnumerable<TourDisponible>>
                {
                    Success = true,
                    Message = "Tours disponibles obtenidos exitosamente",
                    Data = tours
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<IEnumerable<TourDisponible>>
                {
                    Success = false,
                    Message = $"Error al obtener tours disponibles: {ex.Message}",
                    Data = new List<TourDisponible>()
                };
            }
        }

        public async Task<ResponseDTO<TourDisponible>> ObtenerTourDisponiblePorId(long idTour)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var tour = await connection.QueryFirstOrDefaultAsync<TourDisponible>(
                    "ConsultarTourDisponiblePorId",
                    new { IdTour = idTour },
                    commandType: CommandType.StoredProcedure
                );

                if (tour == null)
                {
                    return new ResponseDTO<TourDisponible>
                    {
                        Success = false,
                        Message = "Tour no encontrado o no disponible",
                        Data = null
                    };
                }

                return new ResponseDTO<TourDisponible>
                {
                    Success = true,
                    Message = "Tour obtenido exitosamente",
                    Data = tour
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<TourDisponible>
                {
                    Success = false,
                    Message = $"Error al obtener tour: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseDTO<object>> CrearReserva(long idUsuario, CrearReservaDTO dto)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "CrearReserva",
                    new
                    {
                        IdTour = dto.IdTour,
                        IdUsuario = idUsuario,
                        CantidadPersonas = dto.CantidadPersonas,
                        Comentarios = dto.Comentarios
                    },
                    commandType: CommandType.StoredProcedure
                );

                return new ResponseDTO<object>
                {
                    Success = resultado.Resultado > 0,
                    Message = resultado.Mensaje,
                    Data = resultado.Resultado > 0 ? new { IdReserva = resultado.Resultado } : null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<object>
                {
                    Success = false,
                    Message = $"Error al crear reserva: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseDTO<IEnumerable<Reserva>>> ObtenerReservasUsuario(long idUsuario)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var reservas = await connection.QueryAsync<Reserva>(
                    "ConsultarReservasUsuario",
                    new { IdUsuario = idUsuario },
                    commandType: CommandType.StoredProcedure
                );

                return new ResponseDTO<IEnumerable<Reserva>>
                {
                    Success = true,
                    Message = "Reservas obtenidas exitosamente",
                    Data = reservas
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<IEnumerable<Reserva>>
                {
                    Success = false,
                    Message = $"Error al obtener reservas: {ex.Message}",
                    Data = new List<Reserva>()
                };
            }
        }

        public async Task<ResponseDTO<object>> CancelarReserva(long idReserva, long idUsuario)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "CancelarReserva",
                    new { IdReserva = idReserva, IdUsuario = idUsuario },
                    commandType: CommandType.StoredProcedure
                );

                return new ResponseDTO<object>
                {
                    Success = resultado.Resultado > 0,
                    Message = resultado.Mensaje
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<object>
                {
                    Success = false,
                    Message = $"Error al cancelar reserva: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDTO<EstadisticasReservasDTO>> ObtenerEstadisticas()
        {
            try
            {
                using var connection = _context.CreateConnection();
                var estadisticas = await connection.QueryFirstOrDefaultAsync<EstadisticasReservasDTO>(
                    "ObtenerEstadisticasReservas",
                    commandType: CommandType.StoredProcedure
                );

                return new ResponseDTO<EstadisticasReservasDTO>
                {
                    Success = true,
                    Message = "Estadísticas obtenidas exitosamente",
                    Data = estadisticas ?? new EstadisticasReservasDTO()
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<EstadisticasReservasDTO>
                {
                    Success = false,
                    Message = $"Error al obtener estadísticas: {ex.Message}",
                    Data = new EstadisticasReservasDTO()
                };
            }
        }
    }
}