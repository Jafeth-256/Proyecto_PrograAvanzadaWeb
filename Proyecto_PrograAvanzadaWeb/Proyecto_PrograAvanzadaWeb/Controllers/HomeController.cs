using Microsoft.AspNetCore.Mvc;
using Proyecto_PrograAvanzadaWeb.Models;
using Proyecto_PrograAvanzadaWeb.Services;
using System.Diagnostics;

namespace Proyecto_PrograAvanzadaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApiService _apiService;

        public HomeController(ILogger<HomeController> logger, ApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            // Si ya está logueado, redirigir a Inicio
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Inicio");
            }

            return View(new Usuario()); // Pasar modelo vacío para el login
        }

        #region Login
        [HttpPost]
        public async Task<IActionResult> Login(Usuario modelo)
        {
            try
            {
                // Log para debug
                _logger.LogInformation($"Intento de login para: {modelo?.Correo}");

                if (string.IsNullOrEmpty(modelo?.Correo) || string.IsNullOrEmpty(modelo?.Contrasenna))
                {
                    ViewBag.Error = "Debe ingresar correo y contraseña";
                    return View("Index", modelo);
                }

                var loginDto = new LoginDto
                {
                    Correo = modelo.Correo,
                    Contrasenna = modelo.Contrasenna
                };

                _logger.LogInformation($"Enviando request a API para: {loginDto.Correo}");

                var response = await _apiService.Login(loginDto);

                _logger.LogInformation($"Respuesta de API - Success: {response.Success}, Message: {response.Message}");

                if (response.Success && response.Usuario != null)
                {
                    // Guardar datos en sesión
                    HttpContext.Session.SetString("IdUsuario", response.Usuario.IdUsuario.ToString());
                    HttpContext.Session.SetString("Nombre", response.Usuario.Nombre);
                    HttpContext.Session.SetString("Correo", response.Usuario.Correo);
                    HttpContext.Session.SetString("NombreRol", response.Usuario.NombreRol ?? "Usuario Regular");
                    HttpContext.Session.SetString("Token", response.Token ?? "");

                    _logger.LogInformation($"Usuario logueado exitosamente: {response.Usuario.Nombre}");

                    return RedirectToAction("Inicio");
                }
                else
                {
                    ViewBag.Error = response.Message ?? "Error en el login";
                    _logger.LogWarning($"Error en login: {response.Message}");
                    return View("Index", modelo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Excepción en Login: {ex.Message}");
                ViewBag.Error = "Error interno del servidor. Por favor intente nuevamente.";
                return View("Index", modelo);
            }
        }
        #endregion

        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            // Si ya está logueado, redirigir a Inicio
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Inicio");
            }

            return View(new RegistroUsuario());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistroUsuario modelo)
        {
            try
            {
                _logger.LogInformation($"Intento de registro para: {modelo?.Correo}");

                if (string.IsNullOrEmpty(modelo?.Nombre) || string.IsNullOrEmpty(modelo?.Correo) ||
                    string.IsNullOrEmpty(modelo?.Identificacion) || string.IsNullOrEmpty(modelo?.Contrasenna))
                {
                    ViewBag.Error = "Todos los campos son obligatorios";
                    return View(modelo);
                }

                // CORREGIDO: Usar el mismo DTO que espera la API
                var registroDto = new RegistroUsuarioDto
                {
                    Nombre = modelo.Nombre.Trim(),
                    Correo = modelo.Correo.Trim().ToLower(),
                    Identificacion = modelo.Identificacion.Trim(),
                    Contrasenna = modelo.Contrasenna
                };

                _logger.LogInformation($"Enviando datos de registro a API: {registroDto.Correo}");

                var response = await _apiService.Registrar(registroDto);

                _logger.LogInformation($"Respuesta de API registro - Success: {response.Success}, Message: {response.Message}");

                if (response.Success)
                {
                    TempData["Exito"] = "Usuario registrado exitosamente. Puede iniciar sesión.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = response.Message ?? "Error al registrar usuario";
                    _logger.LogWarning($"Error en registro: {response.Message}");
                    return View(modelo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en Register: {ex.Message}");
                ViewBag.Error = "Error interno del servidor. Por favor intente nuevamente.";
                return View(modelo);
            }
        }
        #endregion

        #region Registro (Spanish version)
        [HttpGet]
        public IActionResult Registro()
        {
            // Si ya está logueado, redirigir a Inicio
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Inicio");
            }

            return View(new RegistroUsuario());
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroUsuario modelo)
        {
            try
            {
                _logger.LogInformation($"Intento de registro (ES) para: {modelo?.Correo}");

                if (string.IsNullOrEmpty(modelo?.Nombre) || string.IsNullOrEmpty(modelo?.Correo) ||
                    string.IsNullOrEmpty(modelo?.Identificacion) || string.IsNullOrEmpty(modelo?.Contrasenna))
                {
                    ViewBag.Error = "Todos los campos son obligatorios";
                    return View(modelo);
                }

                // Validaciones adicionales
                if (modelo.Contrasenna.Length < 6)
                {
                    ViewBag.Error = "La contraseña debe tener al menos 6 caracteres";
                    return View(modelo);
                }

                if (modelo.Nombre.Length > 255)
                {
                    ViewBag.Error = "El nombre no puede exceder 255 caracteres";
                    return View(modelo);
                }

                if (modelo.Correo.Length > 100)
                {
                    ViewBag.Error = "El correo no puede exceder 100 caracteres";
                    return View(modelo);
                }

                if (modelo.Identificacion.Length > 20)
                {
                    ViewBag.Error = "La identificación no puede exceder 20 caracteres";
                    return View(modelo);
                }

                // CORREGIDO: Usar el mismo DTO que espera la API
                var registroDto = new RegistroUsuarioDto
                {
                    Nombre = modelo.Nombre.Trim(),
                    Correo = modelo.Correo.Trim().ToLower(),
                    Identificacion = modelo.Identificacion.Trim(),
                    Contrasenna = modelo.Contrasenna
                };

                _logger.LogInformation($"Enviando datos de registro a API: {registroDto.Correo}");

                var response = await _apiService.Registrar(registroDto);

                _logger.LogInformation($"Respuesta de API registro - Success: {response.Success}, Message: {response.Message}");

                if (response.Success)
                {
                    TempData["Exito"] = "Usuario registrado exitosamente. Puede iniciar sesión.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = response.Message ?? "Error al registrar usuario";
                    _logger.LogWarning($"Error en registro: {response.Message}");
                    return View(modelo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en Registro: {ex.Message}");
                ViewBag.Error = "Error interno del servidor. Por favor intente nuevamente.";
                return View(modelo);
            }
        }
        #endregion

        #region Recuperar Contraseña
        [HttpGet]
        public IActionResult RecuperarContrasena()
        {
            // Si ya está logueado, redirigir a Inicio
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Inicio");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarContrasena(string correo)
        {
            try
            {
                if (string.IsNullOrEmpty(correo))
                {
                    ViewBag.Error = "El correo electrónico es obligatorio";
                    return View();
                }

                // Aquí llamarías a tu API para enviar el email de recuperación
                // var response = await _apiService.RecuperarContrasena(correo);

                // Por ahora mostrar mensaje de éxito genérico
                ViewBag.Exito = "Si el correo existe en nuestro sistema, recibirás instrucciones para restablecer tu contraseña.";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en RecuperarContrasena: {ex.Message}");
                ViewBag.Error = "Error interno del servidor. Por favor intente nuevamente.";
                return View();
            }
        }
        #endregion

        #region Restablecer Contraseña
        [HttpGet]
        public IActionResult RestablecerContrasena(string token)
        {
            // Si ya está logueado, redirigir a Inicio
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Inicio");
            }

            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Token inválido o expirado";
                return RedirectToAction("Index");
            }

            ViewBag.Token = token;
            // Aquí podrías validar el token y obtener el email asociado
            // var emailFromToken = await _apiService.ValidarToken(token);
            // ViewBag.Correo = emailFromToken;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerContrasena(string token, string nuevaContrasena, string confirmarContrasena)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    ViewBag.Error = "Token inválido";
                    ViewBag.Token = token;
                    return View();
                }

                if (string.IsNullOrEmpty(nuevaContrasena) || nuevaContrasena.Length < 6)
                {
                    ViewBag.Error = "La contraseña debe tener al menos 6 caracteres";
                    ViewBag.Token = token;
                    return View();
                }

                if (nuevaContrasena != confirmarContrasena)
                {
                    ViewBag.Error = "Las contraseñas no coinciden";
                    ViewBag.Token = token;
                    return View();
                }

                // Aquí llamarías a tu API para restablecer la contraseña
                // var response = await _apiService.RestablecerContrasena(token, nuevaContrasena);

                TempData["Exito"] = "Tu contraseña ha sido restablecida exitosamente. Ya puedes iniciar sesión.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en RestablecerContrasena: {ex.Message}");
                ViewBag.Error = "Error interno del servidor. Por favor intente nuevamente.";
                ViewBag.Token = token;
                return View();
            }
        }
        #endregion

        #region Inicio (Página principal después del login)
        public IActionResult Inicio()
        {
            // Verificar que esté logueado
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index");
            }

            // Pasar datos del usuario a la vista
            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");
            ViewBag.IdUsuario = HttpContext.Session.GetString("IdUsuario");
            ViewBag.Correo = HttpContext.Session.GetString("Correo");

            return View();
        }
        #endregion

        #region Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        #endregion

        #region Métodos auxiliares para verificar sesión
        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario"));
        }

        private void SetUserViewData()
        {
            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");
            ViewBag.IdUsuario = HttpContext.Session.GetString("IdUsuario");
            ViewBag.Correo = HttpContext.Session.GetString("Correo");
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}