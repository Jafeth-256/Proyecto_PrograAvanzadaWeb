using Proyecto_PrograAvanzadaWeb.Models;

namespace Proyecto_PrograAvanzadaWeb.Services
{
    public interface IApiService
    {
        Task<ApiResponse<List<UsuarioViewModel>>> ObtenerTodosLosUsuariosAsync();
        Task<ApiResponse<UsuarioViewModel>> ObtenerUsuarioPorIdAsync(long id);
        Task<ApiResponse<bool>> ActualizarUsuarioAsync(long id, UsuarioViewModel usuario);
        Task<ApiResponse<bool>> CambiarEstadoUsuarioAsync(long id, bool nuevoEstado);
        Task<ApiResponse<EstadisticasUsuariosViewModel>> ObtenerEstadisticasUsuariosAsync();
    }


    public class EstadisticasUsuariosViewModel
    {
        public int TotalUsuarios { get; set; }
        public int UsuariosActivos { get; set; }
        public int UsuariosInactivos { get; set; }
        public int UsuariosRegulares { get; set; }
        public int UsuariosAdministradores { get; set; }
    }
}