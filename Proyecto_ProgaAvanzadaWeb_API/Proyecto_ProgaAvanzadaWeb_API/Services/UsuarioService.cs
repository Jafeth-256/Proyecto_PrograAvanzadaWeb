using Proyecto_ProgaAvanzadaWeb_API.Helpers;
using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;
using Proyecto_ProgaAvanzadaWeb_API.Models.Entities;
using Proyecto_PrograAvanzadaWeb_API.Data;
using System.Data;
using Dapper;

namespace Proyecto_ProgaAvanzadaWeb_API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly DataContext _context;

        public UsuarioService(DataContext context)
        {
            _context = context;
        }

        public async Task<ResponseDTO<List<UsuarioDTO>>> ObtenerTodos()
        {
            try
            {
                using var connection = _context.CreateConnection();

                var usuarios = await connection.QueryAsync<Usuario>(
                    "ObtenerTodosLosUsuarios",
                    commandType: CommandType.StoredProcedure
                );

                var usuariosDto = usuarios.Select(u => new UsuarioDTO
                {
                    IdUsuario = u.IdUsuario,
                    Nombre = u.Nombre,
                    Correo = u.Correo,
                    Identificacion = u.Identificacion,
                    Estado = u.Estado,
                    IdRol = u.IdRol,
                    NombreRol = u.NombreRol,
                    Telefono = u.Telefono,
                    Direccion = u.Direccion,
                    FechaNacimiento = u.FechaNacimiento,
                    FotoPath = u.FotoPath
                }).ToList();

                return new ResponseDTO<List<UsuarioDTO>>
                {
                    Success = true,
                    Data = usuariosDto
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<UsuarioDTO>>
                {
                    Success = false,
                    Message = $"Error al obtener usuarios: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDTO<UsuarioDTO>> ObtenerPorId(long id)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                    "ObtenerUsuarioPorId",
                    new { IdUsuario = id },
                    commandType: CommandType.StoredProcedure
                );

                if (usuario == null)
                {
                    return new ResponseDTO<UsuarioDTO>
                    {
                        Success = false,
                        Message = "Usuario no encontrado"
                    };
                }

                var usuarioDto = new UsuarioDTO
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
                    FotoPath = usuario.FotoPath
                };

                return new ResponseDTO<UsuarioDTO>
                {
                    Success = true,
                    Data = usuarioDto
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<UsuarioDTO>
                {
                    Success = false,
                    Message = $"Error al obtener usuario: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDTO<bool>> ActualizarPerfil(long idUsuario, ActualizarPerfilDTO dto)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "ActualizarPerfilBasico",
                    new
                    {
                        IdUsuario = idUsuario,
                        Nombre = dto.Nombre,
                        Correo = dto.Correo,
                        Identificacion = dto.Identificacion
                    },
                    commandType: CommandType.StoredProcedure
                );

                return new ResponseDTO<bool>
                {
                    Success = resultado.Resultado > 0,
                    Message = resultado.Mensaje,
                    Data = resultado.Resultado > 0
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>
                {
                    Success = false,
                    Message = $"Error al actualizar perfil: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDTO<bool>> CambiarContrasena(long idUsuario, CambiarContrasenaDTO dto)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var contrasenaActualEncriptada = PasswordHelper.EncriptarContrasena(dto.ContrasenaActual);
                var contrasenaNuevaEncriptada = PasswordHelper.EncriptarContrasena(dto.ContrasenaNueva);

                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "CambiarContrasena",
                    new
                    {
                        IdUsuario = idUsuario,
                        ContrasenaActual = contrasenaActualEncriptada,
                        ContrasenaNueva = contrasenaNuevaEncriptada
                    },
                    commandType: CommandType.StoredProcedure
                );

                return new ResponseDTO<bool>
                {
                    Success = resultado.Resultado > 0,
                    Message = resultado.Mensaje,
                    Data = resultado.Resultado > 0
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>
                {
                    Success = false,
                    Message = $"Error al cambiar contraseña: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDTO<bool>> CambiarEstado(long idUsuario, bool nuevoEstado)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "CambiarEstadoUsuario",
                    new { IdUsuario = idUsuario, Estado = nuevoEstado },
                    commandType: CommandType.StoredProcedure
                );

                return new ResponseDTO<bool>
                {
                    Success = resultado.Resultado > 0,
                    Message = resultado.Mensaje,
                    Data = resultado.Resultado > 0
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>
                {
                    Success = false,
                    Message = $"Error al cambiar estado: {ex.Message}"
                };
            }
        }
    }

}
