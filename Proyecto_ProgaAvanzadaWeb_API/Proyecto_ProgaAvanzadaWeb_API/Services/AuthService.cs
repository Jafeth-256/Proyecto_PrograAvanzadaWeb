using Proyecto_ProgaAvanzadaWeb_API.Helpers;
using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;
using Proyecto_ProgaAvanzadaWeb_API.Models.Entities;
using Proyecto_PrograAvanzadaWeb_API.Data;
using System.Data;
using Dapper;

namespace Proyecto_ProgaAvanzadaWeb_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(DataContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
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

                var token = _jwtHelper.GenerateToken(usuario);

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
                        NombreRol = usuario.NombreRol,
                        Telefono = usuario.Telefono,
                        Direccion = usuario.Direccion,
                        FechaNacimiento = usuario.FechaNacimiento,
                        FotoPath = usuario.FotoPath,
                        FechaRegistro = usuario.FechaRegistro,
                        FechaActualizacion = usuario.FechaActualizacion
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

                // CORREGIDO: Convertir decimal a long explícitamente
                long resultadoId = 0;
                if (resultado.Resultado is decimal decimalResult)
                {
                    resultadoId = (long)decimalResult;
                }
                else if (resultado.Resultado is int intResult)
                {
                    resultadoId = intResult;
                }
                else if (resultado.Resultado is long longResult)
                {
                    resultadoId = longResult;
                }

                bool esExitoso = resultadoId > 0;
                string mensaje = resultado.Mensaje?.ToString() ?? "Sin mensaje";

                return new ResponseDTO<UsuarioDTO>
                {
                    Success = esExitoso,
                    Message = mensaje,
                    Data = esExitoso ? new UsuarioDTO
                    {
                        IdUsuario = resultadoId,
                        Nombre = registroDto.Nombre,
                        Correo = registroDto.Correo,
                        Identificacion = registroDto.Identificacion,
                        Estado = true,
                        IdRol = 1,
                        NombreRol = "Usuario Regular",
                        FechaRegistro = DateTime.Now,
                        FechaActualizacion = DateTime.Now
                    } : null
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