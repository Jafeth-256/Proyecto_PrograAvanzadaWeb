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

        #region Métodos de Tours

        /// <summary>
        /// Obtiene todos los tours disponibles
        /// </summary>
        public async Task<ApiResponse<List<TourDto>>> ObtenerTodosLosTours()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/tours");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ApiResponse<List<TourDto>>>(content, _jsonOptions);
                    }
                    catch (JsonException ex)
                    {
                        // Manejar respuesta en formato diferente
                        var toursList = JsonSerializer.Deserialize<List<TourDto>>(content, _jsonOptions);
                        return new ApiResponse<List<TourDto>>
                        {
                            Success = true,
                            Message = "Tours obtenidos exitosamente",
                            Data = toursList ?? new List<TourDto>()
                        };
                    }
                }

                return new ApiResponse<List<TourDto>>
                {
                    Success = false,
                    Message = "Error al obtener los tours",
                    Data = new List<TourDto>()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TourDto>>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Data = new List<TourDto>()
                };
            }
        }

        /// <summary>
        /// Obtiene un tour por su ID
        /// </summary>
        public async Task<ApiResponse<TourDto>> ObtenerTourPorId(long id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/tours/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ApiResponse<TourDto>>(content, _jsonOptions);
                    }
                    catch (JsonException)
                    {
                        // Manejar respuesta en formato diferente
                        var tour = JsonSerializer.Deserialize<TourDto>(content, _jsonOptions);
                        return new ApiResponse<TourDto>
                        {
                            Success = true,
                            Message = "Tour obtenido exitosamente",
                            Data = tour
                        };
                    }
                }

                return new ApiResponse<TourDto>
                {
                    Success = false,
                    Message = "Tour no encontrado"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TourDto>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Crea un nuevo tour - Solo administradores
        /// </summary>
        public async Task<ApiResponse<bool>> CrearTour(CrearTourDto dto)
        {
            try
            {
                ConfigureAuthHeaders();
                var json = JsonSerializer.Serialize(dto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/tours", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        // Intentar deserializar como ApiResponse<bool>
                        return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                    }
                    catch (JsonException)
                    {
                        try
                        {
                            // Intentar deserializar como ResponseDTO<object> que devuelve la API
                            var apiResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                            return new ApiResponse<bool>
                            {
                                Success = apiResult.Success,
                                Message = apiResult.Message,
                                Data = apiResult.Success
                            };
                        }
                        catch (JsonException)
                        {
                            // Si todo falla, asumir éxito si el status code es exitoso
                            return new ApiResponse<bool>
                            {
                                Success = true,
                                Message = "Tour creado exitosamente",
                                Data = true
                            };
                        }
                    }
                }
                else
                {
                    try
                    {
                        var errorResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = errorResult.Message ?? "Error al crear el tour",
                            Data = false
                        };
                    }
                    catch (JsonException)
                    {
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = "Error al crear el tour",
                            Data = false
                        };
                    }
                }
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

        /// <summary>
        /// Actualiza un tour existente - Solo administradores
        /// </summary>
        public async Task<ApiResponse<bool>> ActualizarTour(long id, CrearTourDto dto)
        {
            try
            {
                ConfigureAuthHeaders();
                var json = JsonSerializer.Serialize(dto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/tours/{id}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                    }
                    catch (JsonException)
                    {
                        try
                        {
                            var apiResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                            return new ApiResponse<bool>
                            {
                                Success = apiResult.Success,
                                Message = apiResult.Message,
                                Data = apiResult.Success
                            };
                        }
                        catch (JsonException)
                        {
                            return new ApiResponse<bool>
                            {
                                Success = true,
                                Message = "Tour actualizado exitosamente",
                                Data = true
                            };
                        }
                    }
                }
                else
                {
                    try
                    {
                        var errorResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = errorResult.Message ?? "Error al actualizar el tour",
                            Data = false
                        };
                    }
                    catch (JsonException)
                    {
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = "Error al actualizar el tour",
                            Data = false
                        };
                    }
                }
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

        /// <summary>
        /// Elimina un tour (eliminación lógica) - Solo administradores
        /// </summary>
        public async Task<ApiResponse<bool>> EliminarTour(long id)
        {
            try
            {
                ConfigureAuthHeaders();
                var response = await _httpClient.DeleteAsync($"api/tours/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                    }
                    catch (JsonException)
                    {
                        try
                        {
                            var apiResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                            return new ApiResponse<bool>
                            {
                                Success = apiResult.Success,
                                Message = apiResult.Message,
                                Data = apiResult.Success
                            };
                        }
                        catch (JsonException)
                        {
                            return new ApiResponse<bool>
                            {
                                Success = true,
                                Message = "Tour eliminado exitosamente",
                                Data = true
                            };
                        }
                    }
                }
                else
                {
                    try
                    {
                        var errorResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = errorResult.Message ?? "Error al eliminar el tour",
                            Data = false
                        };
                    }
                    catch (JsonException)
                    {
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = "Error al eliminar el tour",
                            Data = false
                        };
                    }
                }
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

        #endregion

        #region Métodos de Reservas

        public async Task<ApiResponse<List<TourDisponibleDto>>> ObtenerToursDisponibles()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/reservas/tours-disponibles");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ApiResponse<List<TourDisponibleDto>>>(content, _jsonOptions);
                    }
                    catch (JsonException)
                    {
                        var toursList = JsonSerializer.Deserialize<List<TourDisponibleDto>>(content, _jsonOptions);
                        return new ApiResponse<List<TourDisponibleDto>>
                        {
                            Success = true,
                            Message = "Tours disponibles obtenidos exitosamente",
                            Data = toursList ?? new List<TourDisponibleDto>()
                        };
                    }
                }

                return new ApiResponse<List<TourDisponibleDto>>
                {
                    Success = false,
                    Message = "Error al obtener los tours disponibles",
                    Data = new List<TourDisponibleDto>()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TourDisponibleDto>>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Data = new List<TourDisponibleDto>()
                };
            }
        }

        public async Task<ApiResponse<TourDisponibleDto>> ObtenerTourDisponiblePorId(long id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/reservas/tours-disponibles/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ApiResponse<TourDisponibleDto>>(content, _jsonOptions);
                    }
                    catch (JsonException)
                    {
                        var tour = JsonSerializer.Deserialize<TourDisponibleDto>(content, _jsonOptions);
                        return new ApiResponse<TourDisponibleDto>
                        {
                            Success = true,
                            Message = "Tour obtenido exitosamente",
                            Data = tour
                        };
                    }
                }

                return new ApiResponse<TourDisponibleDto>
                {
                    Success = false,
                    Message = "Tour no encontrado"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TourDisponibleDto>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> CrearReserva(CrearReservaDto dto)
        {
            try
            {
                ConfigureAuthHeaders();
                var json = JsonSerializer.Serialize(dto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/reservas", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                    }
                    catch (JsonException)
                    {
                        try
                        {
                            var apiResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                            return new ApiResponse<bool>
                            {
                                Success = apiResult.Success,
                                Message = apiResult.Message,
                                Data = apiResult.Success
                            };
                        }
                        catch (JsonException)
                        {
                            return new ApiResponse<bool>
                            {
                                Success = true,
                                Message = "Reserva creada exitosamente",
                                Data = true
                            };
                        }
                    }
                }
                else
                {
                    try
                    {
                        var errorResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = errorResult.Message ?? "Error al crear la reserva",
                            Data = false
                        };
                    }
                    catch (JsonException)
                    {
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = "Error al crear la reserva",
                            Data = false
                        };
                    }
                }
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

        public async Task<ApiResponse<List<ReservaDto>>> ObtenerMisReservas()
        {
            try
            {
                ConfigureAuthHeaders();
                var response = await _httpClient.GetAsync("api/reservas/mis-reservas");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ApiResponse<List<ReservaDto>>>(content, _jsonOptions);
                    }
                    catch (JsonException)
                    {
                        var reservasList = JsonSerializer.Deserialize<List<ReservaDto>>(content, _jsonOptions);
                        return new ApiResponse<List<ReservaDto>>
                        {
                            Success = true,
                            Message = "Reservas obtenidas exitosamente",
                            Data = reservasList ?? new List<ReservaDto>()
                        };
                    }
                }

                return new ApiResponse<List<ReservaDto>>
                {
                    Success = false,
                    Message = "Error al obtener las reservas",
                    Data = new List<ReservaDto>()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ReservaDto>>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Data = new List<ReservaDto>()
                };
            }
        }

        public async Task<ApiResponse<bool>> CancelarReserva(long idReserva)
        {
            try
            {
                ConfigureAuthHeaders();
                var response = await _httpClient.PostAsync($"api/reservas/{idReserva}/cancelar", null);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                    }
                    catch (JsonException)
                    {
                        try
                        {
                            var apiResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                            return new ApiResponse<bool>
                            {
                                Success = apiResult.Success,
                                Message = apiResult.Message,
                                Data = apiResult.Success
                            };
                        }
                        catch (JsonException)
                        {
                            return new ApiResponse<bool>
                            {
                                Success = true,
                                Message = "Reserva cancelada exitosamente",
                                Data = true
                            };
                        }
                    }
                }
                else
                {
                    try
                    {
                        var errorResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = errorResult.Message ?? "Error al cancelar la reserva",
                            Data = false
                        };
                    }
                    catch (JsonException)
                    {
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = "Error al cancelar la reserva",
                            Data = false
                        };
                    }
                }
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

        public async Task<ApiResponse<EstadisticasReservasDto>> ObtenerEstadisticasReservas()
        {
            try
            {
                ConfigureAuthHeaders();
                var response = await _httpClient.GetAsync("api/reservas/estadisticas");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<ApiResponse<EstadisticasReservasDto>>(content, _jsonOptions);
                }

                return new ApiResponse<EstadisticasReservasDto>
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode}",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<EstadisticasReservasDto>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Data = null
                };
            }
        }

        #endregion

        // Extensión del ApiService para agregar métodos de eventos
        // Agregar estas clases al final del archivo ApiService.cs

            public async Task<ApiResponse<List<EventoSocialDto>>> ObtenerTodosLosEventosSociales()
            {
                try
                {
                    var response = await _httpClient.GetAsync("api/eventossociales");
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            return JsonSerializer.Deserialize<ApiResponse<List<EventoSocialDto>>>(content, _jsonOptions);
                        }
                        catch (JsonException)
                        {
                            var eventosList = JsonSerializer.Deserialize<List<EventoSocialDto>>(content, _jsonOptions);
                            return new ApiResponse<List<EventoSocialDto>>
                            {
                                Success = true,
                                Message = "Eventos sociales obtenidos exitosamente",
                                Data = eventosList ?? new List<EventoSocialDto>()
                            };
                        }
                    }

                    return new ApiResponse<List<EventoSocialDto>>
                    {
                        Success = false,
                        Message = "Error al obtener los eventos sociales",
                        Data = new List<EventoSocialDto>()
                    };
                }
                catch (Exception ex)
                {
                    return new ApiResponse<List<EventoSocialDto>>
                    {
                        Success = false,
                        Message = $"Error de conexión: {ex.Message}",
                        Data = new List<EventoSocialDto>()
                    };
                }
            }

            /// <summary>
            /// Obtiene un evento social por su ID
            /// </summary>
            public async Task<ApiResponse<EventoSocialDto>> ObtenerEventoSocialPorId(long id)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"api/eventossociales/{id}");
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            return JsonSerializer.Deserialize<ApiResponse<EventoSocialDto>>(content, _jsonOptions);
                        }
                        catch (JsonException)
                        {
                            var evento = JsonSerializer.Deserialize<EventoSocialDto>(content, _jsonOptions);
                            return new ApiResponse<EventoSocialDto>
                            {
                                Success = true,
                                Message = "Evento social obtenido exitosamente",
                                Data = evento
                            };
                        }
                    }

                    return new ApiResponse<EventoSocialDto>
                    {
                        Success = false,
                        Message = "Evento social no encontrado"
                    };
                }
                catch (Exception ex)
                {
                    return new ApiResponse<EventoSocialDto>
                    {
                        Success = false,
                        Message = $"Error de conexión: {ex.Message}"
                    };
                }
            }

            /// <summary>
            /// Crea un nuevo evento social - Solo administradores
            /// </summary>
            public async Task<ApiResponse<bool>> CrearEventoSocial(CrearEventoSocialDto dto)
            {
                try
                {
                    ConfigureAuthHeaders();
                    var json = JsonSerializer.Serialize(dto, _jsonOptions);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync("api/eventossociales", content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                        }
                        catch (JsonException)
                        {
                            try
                            {
                                var apiResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                                return new ApiResponse<bool>
                                {
                                    Success = apiResult.Success,
                                    Message = apiResult.Message,
                                    Data = apiResult.Success
                                };
                            }
                            catch (JsonException)
                            {
                                return new ApiResponse<bool>
                                {
                                    Success = true,
                                    Message = "Evento social creado exitosamente",
                                    Data = true
                                };
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            var errorResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                            return new ApiResponse<bool>
                            {
                                Success = false,
                                Message = errorResult.Message ?? "Error al crear el evento social",
                                Data = false
                            };
                        }
                        catch (JsonException)
                        {
                            return new ApiResponse<bool>
                            {
                                Success = false,
                                Message = "Error al crear el evento social",
                                Data = false
                            };
                        }
                    }
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

            /// <summary>
            /// Actualiza un evento social existente - Solo administradores
            /// </summary>
            public async Task<ApiResponse<bool>> ActualizarEventoSocial(long id, CrearEventoSocialDto dto)
            {
                try
                {
                    ConfigureAuthHeaders();
                    var json = JsonSerializer.Serialize(dto, _jsonOptions);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PutAsync($"api/eventossociales/{id}", content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                        }
                        catch (JsonException)
                        {
                            try
                            {
                                var apiResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                                return new ApiResponse<bool>
                                {
                                    Success = apiResult.Success,
                                    Message = apiResult.Message,
                                    Data = apiResult.Success
                                };
                            }
                            catch (JsonException)
                            {
                                return new ApiResponse<bool>
                                {
                                    Success = true,
                                    Message = "Evento social actualizado exitosamente",
                                    Data = true
                                };
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            var errorResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                            return new ApiResponse<bool>
                            {
                                Success = false,
                                Message = errorResult.Message ?? "Error al actualizar el evento social",
                                Data = false
                            };
                        }
                        catch (JsonException)
                        {
                            return new ApiResponse<bool>
                            {
                                Success = false,
                                Message = "Error al actualizar el evento social",
                                Data = false
                            };
                        }
                    }
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

            /// <summary>
            /// Elimina un evento social (eliminación lógica) - Solo administradores
            /// </summary>
            public async Task<ApiResponse<bool>> EliminarEventoSocial(long id)
            {
                try
                {
                    ConfigureAuthHeaders();
                    var response = await _httpClient.DeleteAsync($"api/eventossociales/{id}");
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                        }
                        catch (JsonException)
                        {
                            try
                            {
                                var apiResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                                return new ApiResponse<bool>
                                {
                                    Success = apiResult.Success,
                                    Message = apiResult.Message,
                                    Data = apiResult.Success
                                };
                            }
                            catch (JsonException)
                            {
                                return new ApiResponse<bool>
                                {
                                    Success = true,
                                    Message = "Evento social eliminado exitosamente",
                                    Data = true
                                };
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            var errorResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                            return new ApiResponse<bool>
                            {
                                Success = false,
                                Message = errorResult.Message ?? "Error al eliminar el evento social",
                                Data = false
                            };
                        }
                        catch (JsonException)
                        {
                            return new ApiResponse<bool>
                            {
                                Success = false,
                                Message = "Error al eliminar el evento social",
                                Data = false
                            };
                        }
                    }
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

        /// <summary>
        /// Obtiene un evento universitario por su ID
        /// </summary>
        public async Task<ApiResponse<EventoUniversitarioDto>> ObtenerEventoUniversitarioPorId(long id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/eventosuniversitarios/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ApiResponse<EventoUniversitarioDto>>(content, _jsonOptions);
                    }
                    catch (JsonException)
                    {
                        var evento = JsonSerializer.Deserialize<EventoUniversitarioDto>(content, _jsonOptions);
                        return new ApiResponse<EventoUniversitarioDto>
                        {
                            Success = true,
                            Message = "Evento universitario obtenido exitosamente",
                            Data = evento
                        };
                    }
                }

                return new ApiResponse<EventoUniversitarioDto>
                {
                    Success = false,
                    Message = "Evento universitario no encontrado"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<EventoUniversitarioDto>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Actualiza un evento universitario existente - Solo administradores
        /// </summary>
        public async Task<ApiResponse<bool>> ActualizarEventoUniversitario(long id, CrearEventoUniversitarioDto dto)
        {
            try
            {
                ConfigureAuthHeaders();
                var json = JsonSerializer.Serialize(dto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/eventosuniversitarios/{id}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                    }
                    catch (JsonException)
                    {
                        try
                        {
                            var apiResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                            return new ApiResponse<bool>
                            {
                                Success = apiResult.Success,
                                Message = apiResult.Message,
                                Data = apiResult.Success
                            };
                        }
                        catch (JsonException)
                        {
                            return new ApiResponse<bool>
                            {
                                Success = true,
                                Message = "Evento universitario actualizado exitosamente",
                                Data = true
                            };
                        }
                    }
                }
                else
                {
                    try
                    {
                        var errorResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = errorResult.Message ?? "Error al actualizar el evento universitario",
                            Data = false
                        };
                    }
                    catch (JsonException)
                    {
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = "Error al actualizar el evento universitario",
                            Data = false
                        };
                    }
                }
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

        /// <summary>
        /// Elimina un evento universitario (eliminación lógica) - Solo administradores
        /// </summary>
        public async Task<ApiResponse<bool>> EliminarEventoUniversitario(long id)
        {
            try
            {
                ConfigureAuthHeaders();
                var response = await _httpClient.DeleteAsync($"api/eventosuniversitarios/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                    }
                    catch (JsonException)
                    {
                        try
                        {
                            var apiResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                            return new ApiResponse<bool>
                            {
                                Success = apiResult.Success,
                                Message = apiResult.Message,
                                Data = apiResult.Success
                            };
                        }
                        catch (JsonException)
                        {
                            return new ApiResponse<bool>
                            {
                                Success = true,
                                Message = "Evento universitario eliminado exitosamente",
                                Data = true
                            };
                        }
                    }
                }
                else
                {
                    try
                    {
                        var errorResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = errorResult.Message ?? "Error al eliminar el evento universitario",
                            Data = false
                        };
                    }
                    catch (JsonException)
                    {
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = "Error al eliminar el evento universitario",
                            Data = false
                        };
                    }
                }
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


        #region Métodos de Eventos Universitarios

        /// <summary>
        /// Obtiene todos los eventos universitarios disponibles
        /// </summary>
        public async Task<ApiResponse<List<EventoUniversitarioDto>>> ObtenerTodosLosEventosUniversitarios()
            {
                try
                {
                    var response = await _httpClient.GetAsync("api/eventosuniversitarios");
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            return JsonSerializer.Deserialize<ApiResponse<List<EventoUniversitarioDto>>>(content, _jsonOptions);
                        }
                        catch (JsonException)
                        {
                            var eventosList = JsonSerializer.Deserialize<List<EventoUniversitarioDto>>(content, _jsonOptions);
                            return new ApiResponse<List<EventoUniversitarioDto>>
                            {
                                Success = true,
                                Message = "Eventos universitarios obtenidos exitosamente",
                                Data = eventosList ?? new List<EventoUniversitarioDto>()
                            };
                        }
                    }

                    return new ApiResponse<List<EventoUniversitarioDto>>
                    {
                        Success = false,
                        Message = "Error al obtener los eventos universitarios",
                        Data = new List<EventoUniversitarioDto>()
                    };
                }
                catch (Exception ex)
                {
                    return new ApiResponse<List<EventoUniversitarioDto>>
                    {
                        Success = false,
                        Message = $"Error de conexión: {ex.Message}",
                        Data = new List<EventoUniversitarioDto>()
                    };
                }
            }

            /// <summary>
            /// Crea un nuevo evento universitario - Solo administradores
            /// </summary>
            public async Task<ApiResponse<bool>> CrearEventoUniversitario(CrearEventoUniversitarioDto dto)
            {
                try
                {
                    ConfigureAuthHeaders();
                    var json = JsonSerializer.Serialize(dto, _jsonOptions);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync("api/eventosuniversitarios", content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                        }
                        catch (JsonException)
                        {
                            try
                            {
                                var apiResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                                return new ApiResponse<bool>
                                {
                                    Success = apiResult.Success,
                                    Message = apiResult.Message,
                                    Data = apiResult.Success
                                };
                            }
                            catch (JsonException)
                            {
                                return new ApiResponse<bool>
                                {
                                    Success = true,
                                    Message = "Evento universitario creado exitosamente",
                                    Data = true
                                };
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            var errorResult = JsonSerializer.Deserialize<ResponseDto<object>>(responseContent, _jsonOptions);
                            return new ApiResponse<bool>
                            {
                                Success = false,
                                Message = errorResult.Message ?? "Error al crear el evento universitario",
                                Data = false
                            };
                        }
                        catch (JsonException)
                        {
                            return new ApiResponse<bool>
                            {
                                Success = false,
                                Message = "Error al crear el evento universitario",
                                Data = false
                            };
                        }
                    }
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
        #region DTOs de Tours

        public class TourDto
        {
            public long IdTour { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public string Destino { get; set; }
            public decimal Precio { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public int CantidadPersonas { get; set; }
            public string NombreCreador { get; set; }
        }

        public class CrearTourDto
        {
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public string Destino { get; set; }
            public decimal Precio { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public int CantidadPersonas { get; set; }
        }

        #endregion

        public class TourDisponibleDto
        {
            public long IdTour { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public string Destino { get; set; }
            public decimal Precio { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public int CantidadPersonas { get; set; }
            public string NombreCreador { get; set; }
            public int PersonasReservadas { get; set; }
            public int CuposDisponibles { get; set; }
        }

        public class CrearReservaDto
        {
            public long IdTour { get; set; }
            public int CantidadPersonas { get; set; }
            public string Comentarios { get; set; }
        }

        public class ReservaDto
        {
            public long IdReserva { get; set; }
            public long IdTour { get; set; }
            public string NombreTour { get; set; }
            public string Destino { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public int CantidadPersonas { get; set; }
            public decimal PrecioTotal { get; set; }
            public DateTime FechaReserva { get; set; }
            public string EstadoReserva { get; set; }
            public string Comentarios { get; set; }
            public string NombreCreador { get; set; }
        }

        public class EstadisticasReservasDto
        {
            public int TotalReservas { get; set; }
            public int ReservasPendientes { get; set; }
            public int ReservasConfirmadas { get; set; }
            public int ReservasCanceladas { get; set; }
            public decimal IngresosTotales { get; set; }
            public int PersonasTotales { get; set; }
        }

        #region DTOs para Eventos

        public class EventoSocialDto
        {
            public long IdEventoSocial { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public string Ubicacion { get; set; }
            public decimal Precio { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public int CantidadPersonas { get; set; }
            public string NombreCreador { get; set; }
        }

        public class CrearEventoSocialDto
        {
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public string Ubicacion { get; set; }
            public decimal Precio { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public int CantidadPersonas { get; set; }
        }

        public class EventoUniversitarioDto
        {
            public long IdEventoUniversitario { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public string Ubicacion { get; set; }
            public string Universidad { get; set; }
            public decimal Precio { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public int CantidadPersonas { get; set; }
            public string NombreCreador { get; set; }
        }

        public class CrearEventoUniversitarioDto
        {
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public string Ubicacion { get; set; }
            public string Universidad { get; set; }
            public decimal Precio { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public int CantidadPersonas { get; set; }
        }

        #endregion
    }