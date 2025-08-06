using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;

namespace Proyecto_ProgaAvanzadaWeb_API.Services
{
    public interface IUsuarioService
    {
        Task<ResponseDTO<List<UsuarioDTO>>> ObtenerTodos();
        Task<ResponseDTO<UsuarioDTO>> ObtenerPorId(long id);
        Task<ResponseDTO<bool>> ActualizarPerfil(long idUsuario, ActualizarPerfilDTO dto);
        Task<ResponseDTO<bool>> CambiarContrasena(long idUsuario, CambiarContrasenaDTO dto);
        Task<ResponseDTO<bool>> CambiarEstado(long idUsuario, bool nuevoEstado);
    }
}
