using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Proyecto_PrograAvanzadaWeb.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            if (!string.IsNullOrEmpty(apiBaseUrl))
            {
                _httpClient.BaseAddress = new Uri(apiBaseUrl);
            }

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        private void ConfigureAuthHeaders()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        #region Métodos de Usuario

        public async Task<ApiResponse<List<UsuarioDto>>> ObtenerTodosLosUsuarios()
        {
            try
            {
                ConfigureAuthHeaders();
                var response = await _httpClient.GetAsync("api/usuarios");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<List<UsuarioDto>>>(content, _jsonOptions);
                }

                return new ApiResponse<List<UsuarioDto>>
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode}",
                    Data = new List<UsuarioDto>()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<UsuarioDto>>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Data = new List<UsuarioDto>()
                };
            }
        }

        public async Task<ApiResponse<UsuarioDto>> ObtenerUsuarioPorId(long id)
        {
            try
            {
                ConfigureAuthHeaders();
                var response = await _httpClient.GetAsync($"api/usuarios/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<UsuarioDto>>(content, _jsonOptions);
                }

                return new ApiResponse<UsuarioDto>
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode}",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UsuarioDto>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<bool>> CambiarEstadoUsuario(long id, bool nuevoEstado)
        {
            try
            {
                ConfigureAuthHeaders();
                var data = new { Estado = nuevoEstado };
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/usuarios/{id}/estado", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                }

                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode}",
                    Data = false
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ApiResponse<bool>> ActualizarUsuario(long id, ActualizarUsuarioCompletoDto dto)
        {
            try
            {
                ConfigureAuthHeaders();
                var json = JsonSerializer.Serialize(dto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/usuarios/{id}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                }

                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode}",
                    Data = false
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ApiResponse<EstadisticasUsuariosDto>> ObtenerEstadisticas()
        {
            try
            {
                ConfigureAuthHeaders();
                var response = await _httpClient.GetAsync("api/usuarios/estadisticas");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<EstadisticasUsuariosDto>>(content, _jsonOptions);
                }

                return new ApiResponse<EstadisticasUsuariosDto>
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode}",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<EstadisticasUsuariosDto>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Data = null
                };
            }
        }

        #endregion

        #region Métodos de Perfil

        public async Task<ApiResponse<PerfilCompletoDto>> ObtenerPerfilCompleto()
        {
            try
            {
                ConfigureAuthHeaders();
                var response = await _httpClient.GetAsync("api/perfil");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<PerfilCompletoDto>>(content, _jsonOptions);
                }

                var errorResponse = JsonSerializer.Deserialize<ApiResponse<PerfilCompletoDto>>(content, _jsonOptions);
                return errorResponse ?? new ApiResponse<PerfilCompletoDto>
                {
                    Success = false,
                    Message = "Error al obtener el perfil"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PerfilCompletoDto>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> ActualizarPerfilBasico(ActualizarPerfilBasicoDto dto)
        {
            try
            {
                ConfigureAuthHeaders();
                var json = JsonSerializer.Serialize(dto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("api/perfil/basico", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                }

                var errorResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                return errorResponse ?? new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error al actualizar el perfil básico"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> ActualizarInformacionAdicional(ActualizarInformacionAdicionalDto dto)
        {
            try
            {
                ConfigureAuthHeaders();
                var json = JsonSerializer.Serialize(dto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("api/perfil/adicional", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                }

                var errorResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                return errorResponse ?? new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error al actualizar la información adicional"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> CambiarContrasena(CambiarContrasenaPerfilDto dto)
        {
            try
            {
                ConfigureAuthHeaders();
                var json = JsonSerializer.Serialize(dto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/perfil/cambiar-contrasena", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                }

                var errorResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                return errorResponse ?? new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error al cambiar la contraseña"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<FotoPerfilDto>> SubirFotoPerfil(IFormFile foto)
        {
            try
            {
                ConfigureAuthHeaders();

                using var content = new MultipartFormDataContent();
                using var streamContent = new StreamContent(foto.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(foto.ContentType);
                content.Add(streamContent, "foto", foto.FileName);

                var response = await _httpClient.PostAsync("api/perfil/subir-foto", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<FotoPerfilDto>>(responseContent, _jsonOptions);
                }

                var errorResponse = JsonSerializer.Deserialize<ApiResponse<FotoPerfilDto>>(responseContent, _jsonOptions);
                return errorResponse ?? new ApiResponse<FotoPerfilDto>
                {
                    Success = false,
                    Message = "Error al subir la foto de perfil"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<FotoPerfilDto>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}"
                };
            }
        }

        #endregion

        #region Métodos de Autenticación

        public async Task<LoginResponse> Login(LoginDto loginDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(loginDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                try
                {
                    return JsonSerializer.Deserialize<LoginResponse>(responseContent, _jsonOptions);
                }
                catch
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return new LoginResponse
                        {
                            Success = true,
                            Message = "Login exitoso",
                            Token = null,
                            Usuario = null
                        };
                    }
                    else
                    {
                        return new LoginResponse
                        {
                            Success = false,
                            Message = "Error en el login",
                            Token = null,
                            Usuario = null
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Token = null,
                    Usuario = null
                };
            }
        }

        public async Task<ResponseDto<object>> Registrar(RegistroUsuarioDto registroDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(registroDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/auth/registrar", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                try
                {
                    var apiResponse = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                    return apiResponse;
                }
                catch
                {

                    if (response.IsSuccessStatusCode)
                    {
                        return new ResponseDto<object>
                        {
                            Success = true,
                            Message = "Usuario registrado exitosamente",
                            Data = null
                        };
                    }
                    else
                    {
                        return new ResponseDto<object>
                        {
                            Success = false,
                            Message = "Error al registrar usuario",
                            Data = null
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Data = null
                };
            }
        }

        #endregion
    }

    #region DTOs para la API

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class ResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public UsuarioDto Usuario { get; set; }
    }

    public class UsuarioDto
    {
        public long IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Identificacion { get; set; }
        public bool Estado { get; set; }
        public int IdRol { get; set; }
        public string NombreRol { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string FotoPath { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }

    public class LoginDto
    {
        public string Correo { get; set; }
        public string Contrasenna { get; set; }
    }

    public class RegistroUsuarioDto
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Identificacion { get; set; }
        public string Contrasenna { get; set; }
    }

    public class ActualizarUsuarioCompletoDto
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Identificacion { get; set; }
        public bool Estado { get; set; }
        public int IdRol { get; set; }
    }

    public class EstadisticasUsuariosDto
    {
        public int TotalUsuarios { get; set; }
        public int UsuariosActivos { get; set; }
        public int UsuariosInactivos { get; set; }
        public int UsuariosRegulares { get; set; }
        public int UsuariosAdministradores { get; set; }
    }

    public class RolDto
    {
        public int IdRol { get; set; }
        public string NombreRol { get; set; }
    }

    #region DTOs de Perfil

    // DTOs para el módulo de perfil
    public class PerfilCompletoDto
    {
        public long IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Identificacion { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string FotoPath { get; set; }
        public bool Estado { get; set; }
        public int IdRol { get; set; }
        public string NombreRol { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }

    public class ActualizarPerfilBasicoDto
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Identificacion { get; set; }
    }

    public class ActualizarInformacionAdicionalDto
    {
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }

    public class CambiarContrasenaPerfilDto
    {
        public string ContrasenaActual { get; set; }
        public string ContrasenaNueva { get; set; }
    }

    public class FotoPerfilDto
    {
        public string FotoPath { get; set; }
    }

    #endregion

    #endregion
}