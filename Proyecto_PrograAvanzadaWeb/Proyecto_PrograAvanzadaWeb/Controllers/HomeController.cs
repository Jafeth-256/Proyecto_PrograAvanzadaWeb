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
                return RedirectToAction("Inicio"); // CAMBIADO: de Dashboard a Inicio
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

                    // CAMBIADO: Redirigir a Inicio después del login exitoso
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
                if (string.IsNullOrEmpty(modelo?.Nombre) || string.IsNullOrEmpty(modelo?.Correo) ||
                    string.IsNullOrEmpty(modelo?.Identificacion) || string.IsNullOrEmpty(modelo?.Contrasenna))
                {
                    ViewBag.Error = "Todos los campos son obligatorios";
                    return View(modelo);
                }

                var registroDto = new RegistroUsuarioDto
                {
                    Nombre = modelo.Nombre,
                    Correo = modelo.Correo,
                    Identificacion = modelo.Identificacion,
                    Contrasenna = modelo.Contrasenna
                };

                var response = await _apiService.Registrar(registroDto);

                if (response.Success)
                {
                    TempData["Exito"] = "Usuario registrado exitosamente. Puede iniciar sesión.";
                    return RedirectToAction("Index"); // Regresar al login
                }
                else
                {
                    ViewBag.Error = response.Message ?? "Error al registrar usuario";
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

        #region Inicio (Página principal después del login)
        public IActionResult Inicio()
        {
            // Verificar que esté logueado
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index"); // CAMBIADO: Regresar al login si no está logueado
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
            return RedirectToAction("Index"); // CAMBIADO: Regresar al login después del logout
        }
        #endregion

        #region Métodos auxiliares para verificar sesión (opcional)
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