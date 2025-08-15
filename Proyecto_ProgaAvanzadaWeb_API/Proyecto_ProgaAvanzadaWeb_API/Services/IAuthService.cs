using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;

namespace Proyecto_ProgaAvanzadaWeb_API.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> Login(LoginDTO loginDto);
        Task<ResponseDTO<UsuarioDTO>> Registrar(RegistroUsuarioDTO registroDto);
    }
}
