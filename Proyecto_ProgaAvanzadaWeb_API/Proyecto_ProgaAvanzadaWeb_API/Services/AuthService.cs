using Proyecto_ProgaAvanzadaWeb_API.Helpers;
using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;
using Proyecto_ProgaAvanzadaWeb_API.Models.Entities;
using Proyecto_PrograAvanzadaWeb_API.Data;
using System.Data;
using Dapper;
using Proyecto_ProgaAvanzadaWeb_API.Services;

namespace Proyecto_ProgaAvanzadaWeb_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDTO> Login(LoginDTO loginDto)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var passwordEncriptada = PasswordHelper.EncriptarContrasena(loginDto.Contrasenna);

                var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                    "ValidarInicioSesion",
                    new { Correo = loginDto.Correo, Contrasenna = passwordEncriptada },
                    commandType: CommandType.StoredProcedure
                );

                if (usuario == null)
                {
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Message = "Credenciales inválidas"
                    };
                }

                var jwtHelper = new JwtHelper(_configuration);
                var token = jwtHelper.GenerateToken(usuario);

                return new LoginResponseDTO
                {
                    Success = true,
                    Message = "Login exitoso",
                    Token = token,
                    Usuario = new UsuarioDTO
                    {
                        IdUsuario = usuario.IdUsuario,
                        Nombre = usuario.Nombre,
                        Correo = usuario.Correo,
                        Identificacion = usuario.Identificacion,
                        Estado = usuario.Estado,
                        IdRol = usuario.IdRol,
                        NombreRol = usuario.NombreRol
                    }
                };
            }
            catch (Exception ex)
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    Message = $"Error en el login: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDTO<UsuarioDTO>> Registrar(RegistroUsuarioDTO registroDto)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var passwordEncriptada = PasswordHelper.EncriptarContrasena(registroDto.Contrasenna);

                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "RegistrarUsuario",
                    new
                    {
                        Nombre = registroDto.Nombre,
                        Correo = registroDto.Correo,
                        Identificacion = registroDto.Identificacion,
                        Contrasenna = passwordEncriptada
                    },
                    commandType: CommandType.StoredProcedure
                );

                return new ResponseDTO<UsuarioDTO>
                {
                    Success = resultado.Resultado > 0,
                    Message = resultado.Mensaje
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<UsuarioDTO>
                {
                    Success = false,
                    Message = $"Error al registrar usuario: {ex.Message}"
                };
            }
        }
    }
}