using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;
using Proyecto_ProgaAvanzadaWeb_API.Models.Entities;

namespace Proyecto_ProgaAvanzadaWeb_API.Services
{
    public interface IReservaSocialService
    {
        Task<ResponseDTO<IEnumerable<EventoSocialDisponibleDTO>>> ObtenerEventosSocialesDisponibles();
        Task<ResponseDTO<EventoSocialDisponibleDTO>> ObtenerEventoSocialDisponiblePorId(long idEventoSocial);
        Task<ResponseDTO<object>> CrearReservaSocial(long idUsuario, CrearReservaSocialDTO dto);
        Task<ResponseDTO<IEnumerable<ReservaSocial>>> ObtenerReservasSocialesUsuario(long idUsuario);
        Task<ResponseDTO<object>> CancelarReservaSocial(long idReservaSocial, long idUsuario);
        Task<ResponseDTO<IEnumerable<ReservaSocial>>> ObtenerTodasLasReservasSociales();
        Task<ResponseDTO<object>> ActualizarEstadoReservaSocialAdmin(long idReservaSocial, string nuevoEstado);
    }
}
