using Microsoft.AspNetCore.Mvc;
using Proyecto_PrograAvanzadaWeb.Services;
using Proyecto_PrograAvanzadaWeb.Models;

namespace Proyecto_PrograAvanzadaWeb.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApiService _apiService;

        public UsuariosController(ApiService apiService)
        {
            _apiService = apiService;
        }

        #region Index - Lista de Usuarios
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            // Verificar que sea administrador
            var nombreRol = HttpContext.Session.GetString("NombreRol");
            if (nombreRol != "Usuario Administrador")
            {
                TempData["Error"] = "No tienes permisos para acceder a esta sección";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = nombreRol;

            var response = await _apiService.ObtenerTodosLosUsuarios();

            if (!response.Success)
            {
                ViewBag.Error = response.Message;
                return View(new List<UsuarioViewModel>());
            }

            // Mapear DTOs a ViewModels
            var usuarios = response.Data.Select(u => new UsuarioViewModel
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
                FotoPath = u.FotoPath,
                FechaRegistro = u.FechaRegistro,
                FechaActualizacion = u.FechaActualizacion
            }).ToList();

            return View(usuarios);
        }
        #endregion

        #region Editar Usuario
        [HttpGet]
        public async Task<IActionResult> Editar(long id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            // Verificar que sea administrador
            var nombreRol = HttpContext.Session.GetString("NombreRol");
            if (nombreRol != "Usuario Administrador")
            {
                TempData["Error"] = "No tienes permisos para acceder a esta sección";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = nombreRol;

            var response = await _apiService.ObtenerUsuarioPorId(id);

            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = "Usuario no encontrado";
                return RedirectToAction("Index");
            }

            // Mapear DTO a ViewModel
            var usuario = new UsuarioViewModel
            {
                IdUsuario = response.Data.IdUsuario,
                Nombre = response.Data.Nombre,
                Correo = response.Data.Correo,
                Identificacion = response.Data.Identificacion,
                Estado = response.Data.Estado,
                IdRol = response.Data.IdRol,
                NombreRol = response.Data.NombreRol,
                Telefono = response.Data.Telefono,
                Direccion = response.Data.Direccion,
                FechaNacimiento = response.Data.FechaNacimiento,
                FotoPath = response.Data.FotoPath,
                FechaRegistro = response.Data.FechaRegistro,
                FechaActualizacion = response.Data.FechaActualizacion
            };

            // Obtener roles para el dropdown (hardcoded porque no tenemos endpoint para roles)
            var roles = new List<RolViewModel>
            {
                new RolViewModel { IdRol = 1, NombreRol = "Usuario Regular" },
                new RolViewModel { IdRol = 2, NombreRol = "Usuario Administrador" }
            };

            ViewBag.Roles = roles;

            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(UsuarioViewModel modelo)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            // Verificar que sea administrador
            var nombreRol = HttpContext.Session.GetString("NombreRol");
            if (nombreRol != "Usuario Administrador")
            {
                TempData["Error"] = "No tienes permisos para realizar esta acción";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = nombreRol;

            if (string.IsNullOrEmpty(modelo.Nombre) || string.IsNullOrEmpty(modelo.Correo) ||
                string.IsNullOrEmpty(modelo.Identificacion))
            {
                ViewBag.Error = "Todos los campos son obligatorios";

                // Recargar roles para el dropdown
                var roles = new List<RolViewModel>
                {
                    new RolViewModel { IdRol = 1, NombreRol = "Usuario Regular" },
                    new RolViewModel { IdRol = 2, NombreRol = "Usuario Administrador" }
                };
                ViewBag.Roles = roles;

                return View(modelo);
            }

            // Mapear ViewModel a DTO
            var dto = new ActualizarUsuarioCompletoDto
            {
                Nombre = modelo.Nombre,
                Correo = modelo.Correo,
                Identificacion = modelo.Identificacion,
                Estado = modelo.Estado,
                IdRol = modelo.IdRol
            };

            var response = await _apiService.ActualizarUsuario(modelo.IdUsuario, dto);

            if (response.Success)
            {
                TempData["Exito"] = "Usuario actualizado exitosamente";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Error = response.Message;

                // Recargar roles para el dropdown
                var roles = new List<RolViewModel>
                {
                    new RolViewModel { IdRol = 1, NombreRol = "Usuario Regular" },
                    new RolViewModel { IdRol = 2, NombreRol = "Usuario Administrador" }
                };
                ViewBag.Roles = roles;

                return View(modelo);
            }
        }
        #endregion

        #region Cambiar Estado (AJAX)
        [HttpPost]
        public async Task<IActionResult> CambiarEstado(long idUsuario, bool nuevoEstado)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return Json(new { success = false, message = "Sesión expirada" });
            }

            // Verificar que sea administrador
            var nombreRol = HttpContext.Session.GetString("NombreRol");
            if (nombreRol != "Usuario Administrador")
            {
                return Json(new { success = false, message = "No tienes permisos para realizar esta acción" });
            }

            var response = await _apiService.CambiarEstadoUsuario(idUsuario, nuevoEstado);

            if (response.Success)
            {
                return Json(new { success = true, message = response.Message });
            }
            else
            {
                return Json(new { success = false, message = response.Message });
            }
        }
        #endregion

        #region Ver Detalles
        public async Task<IActionResult> Detalles(long id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            // Verificar que sea administrador
            var nombreRol = HttpContext.Session.GetString("NombreRol");
            if (nombreRol != "Usuario Administrador")
            {
                TempData["Error"] = "No tienes permisos para acceder a esta sección";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = nombreRol;

            var response = await _apiService.ObtenerUsuarioPorId(id);

            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = "Usuario no encontrado";
                return RedirectToAction("Index");
            }

            // Mapear DTO a ViewModel
            var usuario = new UsuarioViewModel
            {
                IdUsuario = response.Data.IdUsuario,
                Nombre = response.Data.Nombre,
                Correo = response.Data.Correo,
                Identificacion = response.Data.Identificacion,
                Estado = response.Data.Estado,
                IdRol = response.Data.IdRol,
                NombreRol = response.Data.NombreRol,
                Telefono = response.Data.Telefono,
                Direccion = response.Data.Direccion,
                FechaNacimiento = response.Data.FechaNacimiento,
                FotoPath = response.Data.FotoPath,
                FechaRegistro = response.Data.FechaRegistro,
                FechaActualizacion = response.Data.FechaActualizacion
            };

            return View(usuario);
        }
        #endregion

        #region Estadísticas (Nuevo)
        public async Task<IActionResult> Estadisticas()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            // Verificar que sea administrador
            var nombreRol = HttpContext.Session.GetString("NombreRol");
            if (nombreRol != "Usuario Administrador")
            {
                TempData["Error"] = "No tienes permisos para acceder a esta sección";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = nombreRol;

            var response = await _apiService.ObtenerEstadisticas();

            if (!response.Success)
            {
                ViewBag.Error = response.Message;
                return View(new EstadisticasUsuariosDto());
            }

            return View(response.Data);
        }

        // API endpoint para obtener estadísticas via AJAX
        [HttpGet]
        public async Task<IActionResult> ObtenerEstadisticasJson()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return Json(new { success = false, message = "Sesión expirada" });
            }

            var nombreRol = HttpContext.Session.GetString("NombreRol");
            if (nombreRol != "Usuario Administrador")
            {
                return Json(new { success = false, message = "No tienes permisos" });
            }

            var response = await _apiService.ObtenerEstadisticas();

            if (response.Success)
            {
                return Json(new { success = true, data = response.Data });
            }
            else
            {
                return Json(new { success = false, message = response.Message });
            }
        }
        #endregion
    }
}