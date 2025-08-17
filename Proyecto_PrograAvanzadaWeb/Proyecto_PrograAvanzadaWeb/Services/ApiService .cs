using System.Text;
using System.Text.Json;

namespace Proyecto_PrograAvanzadaWeb.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

            // Configurar la base URL de la API
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            if (!string.IsNullOrEmpty(apiBaseUrl))
            {
                _httpClient.BaseAddress = new Uri(apiBaseUrl);
            }
        }

        // Configurar headers con token de autenticación
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
                    return JsonSerializer.Deserialize<ApiResponse<List<UsuarioDto>>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
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
                    return JsonSerializer.Deserialize<ApiResponse<UsuarioDto>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
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
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/usuarios/{id}/estado", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
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
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/usuarios/{id}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
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
                    return JsonSerializer.Deserialize<ApiResponse<EstadisticasUsuariosDto>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
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

        #region Métodos de Autenticación

        public async Task<LoginResponse> Login(LoginDto loginDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(loginDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return new LoginResponse
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode}",
                    Token = null,
                    Usuario = null
                };
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

        public async Task<ApiResponse<UsuarioDto>> Registrar(RegistroUsuarioDto registroDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(registroDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/auth/registrar", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<UsuarioDto>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
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

        #endregion
    }

    #region DTOs para la API

    public class ApiResponse<T>
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

    #endregion
}