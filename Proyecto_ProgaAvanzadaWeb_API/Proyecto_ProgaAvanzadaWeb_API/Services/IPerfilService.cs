using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;

namespace Proyecto_ProgaAvanzadaWeb_API.Services
{
    public interface IPerfilService
    {
        Task<ResponseDTO<PerfilCompletoDTO>> ObtenerPerfilCompleto(long idUsuario);
        Task<ResponseDTO<bool>> ActualizarPerfilBasico(long idUsuario, ActualizarPerfilBasicoDTO dto);
        Task<ResponseDTO<bool>> ActualizarInformacionAdicional(long idUsuario, ActualizarInformacionAdicionalDTO dto);
        Task<ResponseDTO<bool>> CambiarContrasena(long idUsuario, CambiarContrasenaPerfilDTO dto);
        Task<ResponseDTO<FotoPerfilDTO>> SubirFotoPerfil(long idUsuario, IFormFile foto);
    }
}