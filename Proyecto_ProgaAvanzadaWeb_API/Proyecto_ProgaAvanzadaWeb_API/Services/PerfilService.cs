using Proyecto_ProgaAvanzadaWeb_API.Helpers;
using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;
using Proyecto_ProgaAvanzadaWeb_API.Models.Entities;
using Proyecto_PrograAvanzadaWeb_API.Data;
using System.Data;
using Dapper;

namespace Proyecto_ProgaAvanzadaWeb_API.Services
{
    public class PerfilService : IPerfilService
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;

        public PerfilService(DataContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<ResponseDTO<PerfilCompletoDTO>> ObtenerPerfilCompleto(long idUsuario)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var perfil = await connection.QueryFirstOrDefaultAsync<PerfilCompleto>(
                    "ObtenerPerfilUsuario",
                    new { IdUsuario = idUsuario },
                    commandType: CommandType.StoredProcedure
                );

                if (perfil == null)
                {
                    return new ResponseDTO<PerfilCompletoDTO>
                    {
                        Success = false,
                        Message = "Perfil de usuario no encontrado"
                    };
                }

                var perfilDto = new PerfilCompletoDTO
                {
                    IdUsuario = perfil.IdUsuario,
                    Nombre = perfil.Nombre,
                    Correo = perfil.Correo,
                    Identificacion = perfil.Identificacion,
                    Telefono = perfil.Telefono,
                    Direccion = perfil.Direccion,
                    FechaNacimiento = perfil.FechaNacimiento,
                    FotoPath = perfil.FotoPath,
                    Estado = perfil.Estado,
                    IdRol = perfil.IdRol,
                    NombreRol = perfil.NombreRol,
                    FechaRegistro = perfil.FechaRegistro,
                    FechaActualizacion = perfil.FechaActualizacion
                };

                return new ResponseDTO<PerfilCompletoDTO>
                {
                    Success = true,
                    Message = "Perfil obtenido exitosamente",
                    Data = perfilDto
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PerfilCompletoDTO>
                {
                    Success = false,
                    Message = $"Error al obtener perfil: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDTO<bool>> ActualizarPerfilBasico(long idUsuario, ActualizarPerfilBasicoDTO dto)
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
                    Message = $"Error al actualizar perfil básico: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ResponseDTO<bool>> ActualizarInformacionAdicional(long idUsuario, ActualizarInformacionAdicionalDTO dto)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "ActualizarInformacionAdicional",
                    new
                    {
                        IdUsuario = idUsuario,
                        Telefono = dto.Telefono,
                        Direccion = dto.Direccion,
                        FechaNacimiento = dto.FechaNacimiento,
                        FotoPath = (string?)null // No actualizamos foto aquí
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
                    Message = $"Error al actualizar información adicional: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ResponseDTO<bool>> CambiarContrasena(long idUsuario, CambiarContrasenaPerfilDTO dto)
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
                    Message = $"Error al cambiar contraseña: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ResponseDTO<FotoPerfilDTO>> SubirFotoPerfil(long idUsuario, IFormFile foto)
        {
            try
            {
                if (foto == null || foto.Length == 0)
                {
                    return new ResponseDTO<FotoPerfilDTO>
                    {
                        Success = false,
                        Message = "No se ha proporcionado ningún archivo"
                    };
                }

                // Validar extensiones permitidas
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    return new ResponseDTO<FotoPerfilDTO>
                    {
                        Success = false,
                        Message = "Solo se permiten archivos de imagen (jpg, jpeg, png, gif)"
                    };
                }

                // Validar tamaño (5MB máximo)
                if (foto.Length > 5 * 1024 * 1024)
                {
                    return new ResponseDTO<FotoPerfilDTO>
                    {
                        Success = false,
                        Message = "El archivo no puede superar los 5MB"
                    };
                }

                // Crear directorio si no existe
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "profiles");
                Directory.CreateDirectory(uploadsFolder);

                // Generar nombre único para el archivo
                var uniqueFileName = $"{idUsuario}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Guardar archivo
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await foto.CopyToAsync(fileStream);
                }

                var fotoPath = $"/uploads/profiles/{uniqueFileName}";

                // Actualizar ruta en base de datos
                using var connection = _context.CreateConnection();

                var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "ActualizarInformacionAdicional",
                    new
                    {
                        IdUsuario = idUsuario,
                        Telefono = (string?)null,
                        Direccion = (string?)null,
                        FechaNacimiento = (DateTime?)null,
                        FotoPath = fotoPath
                    },
                    commandType: CommandType.StoredProcedure
                );

                if (resultado.Resultado > 0)
                {
                    return new ResponseDTO<FotoPerfilDTO>
                    {
                        Success = true,
                        Message = "Foto de perfil actualizada exitosamente",
                        Data = new FotoPerfilDTO { FotoPath = fotoPath }
                    };
                }
                else
                {
                    // Si falla la actualización, eliminar el archivo
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    return new ResponseDTO<FotoPerfilDTO>
                    {
                        Success = false,
                        Message = "Error al actualizar la foto de perfil en la base de datos"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO<FotoPerfilDTO>
                {
                    Success = false,
                    Message = $"Error al subir foto de perfil: {ex.Message}"
                };
            }
        }
    }
}