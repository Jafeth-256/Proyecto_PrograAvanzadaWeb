using Dapper;
using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;
using Proyecto_ProgaAvanzadaWeb_API.Models.Entities;
using Proyecto_PrograAvanzadaWeb_API.Data;
using System.Data;

namespace Proyecto_ProgaAvanzadaWeb_API.Services
{
    public class ReservaSocialService : IReservaSocialService
    {
        private readonly DataContext _context;

        public ReservaSocialService(DataContext context)
        {
            _context = context;
        }

        public async Task<ResponseDTO<IEnumerable<EventoSocialDisponibleDTO>>> ObtenerEventosSocialesDisponibles()
        {
            try
            {
                using var connection = _context.CreateConnection();
                var eventos = await connection.QueryAsync<EventoSocialDisponibleDTO>(
                    "ConsultarEventosSocialesDisponibles",
                    commandType: CommandType.StoredProcedure
                );

                return new ResponseDTO<IEnumerable<EventoSocialDisponibleDTO>>
                {
                    Success = true,
                    Message = "Eventos sociales disponibles obtenidos exitosamente",
                    Data = eventos
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<IEnumerable<EventoSocialDisponibleDTO>>
                {
                    Success = false,
                    Message = $"Error al obtener eventos sociales disponibles: {ex.Message}",
                    Data = new List<EventoSocialDisponibleDTO>()
                };
            }
        }

        public async Task<ResponseDTO<EventoSocialDisponibleDTO>> ObtenerEventoSocialDisponiblePorId(long idEventoSocial)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var evento = await connection.QueryFirstOrDefaultAsync<EventoSocialDisponibleDTO>(
                    "ConsultarEventoSocialDisponiblePorId",
                    new { IdEventoSocial = idEventoSocial },
                    commandType: CommandType.StoredProcedure
                );

                if (evento == null)
                {
                    return new ResponseDTO<EventoSocialDisponibleDTO>
                    {
                        Success = false,
                        Message = "Evento social no encontrado o no disponible",
                        Data = null
                    };
                }

                return new ResponseDTO<EventoSocialDisponibleDTO>
                {
                    Success = true,
                    Message = "Evento social obtenido exitosamente",
                    Data = evento
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<EventoSocialDisponibleDTO>
                {
                    Success = false,
                    Message = $"Error al obtener evento social: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseDTO<object>> CrearReservaSocial(long idUsuario, CrearReservaSocialDTO dto)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "CrearReservaEventoSocial",
                    new
                    {
                        IdEventoSocial = dto.IdEventoSocial,
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
                    Data = resultado.Resultado > 0 ? new { IdReservaSocial = resultado.Resultado } : null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<object>
                {
                    Success = false,
                    Message = $"Error al crear reserva social: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseDTO<IEnumerable<ReservaSocial>>> ObtenerReservasSocialesUsuario(long idUsuario)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var reservas = await connection.QueryAsync<ReservaSocial>(
                    "ConsultarReservasEventoSocialUsuario",
                    new { IdUsuario = idUsuario },
                    commandType: CommandType.StoredProcedure
                );

                return new ResponseDTO<IEnumerable<ReservaSocial>>
                {
                    Success = true,
                    Message = "Reservas sociales obtenidas exitosamente",
                    Data = reservas
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<IEnumerable<ReservaSocial>>
                {
                    Success = false,
                    Message = $"Error al obtener reservas sociales: {ex.Message}",
                    Data = new List<ReservaSocial>()
                };
            }
        }

        public async Task<ResponseDTO<object>> CancelarReservaSocial(long idReservaSocial, long idUsuario)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "CancelarReservaEventoSocial",
                    new { IdReservaSocial = idReservaSocial, IdUsuario = idUsuario },
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
                    Message = $"Error al cancelar reserva social: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDTO<IEnumerable<ReservaSocial>>> ObtenerTodasLasReservasSociales()
        {
            try
            {
                using var connection = _context.CreateConnection();
                var reservas = await connection.QueryAsync<ReservaSocial>(
                    "ConsultarTodasLasReservasSociales",
                    commandType: CommandType.StoredProcedure
                );

                return new ResponseDTO<IEnumerable<ReservaSocial>>
                {
                    Success = true,
                    Message = "Reservas de eventos sociales obtenidas exitosamente",
                    Data = reservas
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<IEnumerable<ReservaSocial>>
                {
                    Success = false,
                    Message = $"Error al obtener las reservas de eventos sociales: {ex.Message}",
                    Data = new List<ReservaSocial>()
                };
            }
        }

        public async Task<ResponseDTO<object>> ActualizarEstadoReservaSocialAdmin(long idReservaSocial, string nuevoEstado)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "ActualizarEstadoReservaSocialAdmin",
                    new { IdReservaSocial = idReservaSocial, NuevoEstado = nuevoEstado },
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
                    Message = $"Error al actualizar el estado de la reserva social: {ex.Message}"
                };
            }
        }
    }
}
