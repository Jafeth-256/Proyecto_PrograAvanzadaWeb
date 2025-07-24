using Proyecto_PrograAvanzadaWeb.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Proyecto_PrograAvanzadaWeb.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IConfiguration _configuration;

        public UsuariosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Index - Lista de Usuarios
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            List<UsuarioViewModel> usuarios = new List<UsuarioViewModel>();

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                usuarios = context.Query<UsuarioViewModel>(
                    "ObtenerTodosLosUsuarios",
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();
            }

            return View(usuarios);
        }
        #endregion

        #region Editar Usuario
        [HttpGet]
        public IActionResult Editar(long id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            UsuarioViewModel usuario = new UsuarioViewModel();

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                usuario = context.QueryFirstOrDefault<UsuarioViewModel>(
                    "ObtenerUsuarioPorId",
                    new { IdUsuario = id },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (usuario == null)
                {
                    return NotFound();
                }

                // Obtener roles para el dropdown
                var roles = context.Query<RolViewModel>(
                    "SELECT IdRol, NombreRol FROM TRol"
                ).ToList();

                ViewBag.Roles = roles;
            }

            return View(usuario);
        }

        [HttpPost]
        public IActionResult Editar(UsuarioViewModel modelo)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            if (string.IsNullOrEmpty(modelo.Nombre) || string.IsNullOrEmpty(modelo.Correo) ||
                string.IsNullOrEmpty(modelo.Identificacion))
            {
                ViewBag.Error = "Todos los campos son obligatorios";

                // Recargar roles para el dropdown
                using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
                {
                    var roles = context.Query<RolViewModel>(
                        "SELECT IdRol, NombreRol FROM TRol"
                    ).ToList();
                    ViewBag.Roles = roles;
                }

                return View(modelo);
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<dynamic>(
                    "ActualizarUsuario",
                    new
                    {
                        IdUsuario = modelo.IdUsuario,
                        Nombre = modelo.Nombre,
                        Correo = modelo.Correo,
                        Identificacion = modelo.Identificacion,
                        Estado = modelo.Estado,
                        IdRol = modelo.IdRol
                    },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (resultado.Resultado > 0)
                {
                    TempData["Exito"] = "Usuario actualizado exitosamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = resultado.Mensaje;

                    // Recargar roles para el dropdown
                    var roles = context.Query<RolViewModel>(
                        "SELECT IdRol, NombreRol FROM TRol"
                    ).ToList();
                    ViewBag.Roles = roles;

                    return View(modelo);
                }
            }
        }
        #endregion

        #region Cambiar Estado (AJAX)
        [HttpPost]
        public IActionResult CambiarEstado(long idUsuario, bool nuevoEstado)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return Json(new { success = false, message = "Sesión expirada" });
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<dynamic>(
                    "CambiarEstadoUsuario",
                    new { IdUsuario = idUsuario, Estado = nuevoEstado },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (resultado.Resultado > 0)
                {
                    return Json(new { success = true, message = resultado.Mensaje });
                }
                else
                {
                    return Json(new { success = false, message = resultado.Mensaje });
                }
            }
        }
        #endregion

        #region Ver Detalles
        public IActionResult Detalles(long id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            UsuarioViewModel usuario = new UsuarioViewModel();

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                usuario = context.QueryFirstOrDefault<UsuarioViewModel>(
                    "ObtenerUsuarioPorId",
                    new { IdUsuario = id },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (usuario == null)
                {
                    return NotFound();
                }
            }

            return View(usuario);
        }
        #endregion
    }
}