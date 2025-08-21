using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;
using Proyecto_ProgaAvanzadaWeb_API.Models.Entities;

namespace Proyecto_ProgaAvanzadaWeb_API.Services
{
    public interface IReservaService
    {
        Task<ResponseDTO<IEnumerable<TourDisponible>>> ObtenerToursDisponibles();
        Task<ResponseDTO<TourDisponible>> ObtenerTourDisponiblePorId(long idTour);
        Task<ResponseDTO<object>> CrearReserva(long idUsuario, CrearReservaDTO dto);
        Task<ResponseDTO<IEnumerable<Reserva>>> ObtenerReservasUsuario(long idUsuario);
        Task<ResponseDTO<object>> CancelarReserva(long idReserva, long idUsuario);
        Task<ResponseDTO<EstadisticasReservasDTO>> ObtenerEstadisticas();
        Task<ResponseDTO<IEnumerable<Reserva>>> ObtenerTodasLasReservas();
        Task<ResponseDTO<object>> ActualizarEstadoReservaAdmin(long idReserva, string nuevoEstado);
    }
}