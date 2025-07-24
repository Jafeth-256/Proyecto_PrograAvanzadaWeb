using Proyecto_PrograAvanzadaWeb.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace Proyecto_PrograAvanzadaWeb.Controllers
{
    public class PerfilController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public PerfilController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        #region Ver Perfil
        [HttpGet]
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            long idUsuario = Convert.ToInt64(HttpContext.Session.GetString("IdUsuario"));

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var perfil = context.QueryFirstOrDefault<PerfilUsuario>(
                    "ObtenerPerfilUsuario",
                    new { IdUsuario = idUsuario },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (perfil != null)
                {
                    return View(perfil);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        #endregion

        #region Editar Información Básica
        [HttpGet]
        public IActionResult EditarBasico()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            long idUsuario = Convert.ToInt64(HttpContext.Session.GetString("IdUsuario"));

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var perfil = context.QueryFirstOrDefault<PerfilUsuario>(
                    "ObtenerPerfilUsuario",
                    new { IdUsuario = idUsuario },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (perfil != null)
                {
                    var model = new ActualizarPerfilBasico
                    {
                        IdUsuario = perfil.IdUsuario,
                        Nombre = perfil.Nombre,
                        Correo = perfil.Correo,
                        Identificacion = perfil.Identificacion
                    };
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpPost]
        public IActionResult EditarBasico(ActualizarPerfilBasico model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(model.Nombre) || string.IsNullOrEmpty(model.Correo) ||
                string.IsNullOrEmpty(model.Identificacion))
            {
                ViewBag.Error = "Todos los campos son obligatorios";
                return View(model);
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<dynamic>(
                    "ActualizarPerfilBasico",
                    new
                    {
                        IdUsuario = model.IdUsuario,
                        Nombre = model.Nombre,
                        Correo = model.Correo,
                        Identificacion = model.Identificacion
                    },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (resultado.Resultado > 0)
                {
                    // Actualizar datos de sesión
                    HttpContext.Session.SetString("Nombre", model.Nombre);
                    HttpContext.Session.SetString("Correo", model.Correo);

                    ViewBag.Exito = "Perfil actualizado exitosamente";
                    return View(model);
                }
                else
                {
                    ViewBag.Error = resultado.Mensaje;
                    return View(model);
                }
            }
        }
        #endregion

        #region Editar Información Adicional
        [HttpGet]
        public IActionResult EditarAdicional()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            long idUsuario = Convert.ToInt64(HttpContext.Session.GetString("IdUsuario"));

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var perfil = context.QueryFirstOrDefault<PerfilUsuario>(
                    "ObtenerPerfilUsuario",
                    new { IdUsuario = idUsuario },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (perfil != null)
                {
                    var model = new InformacionAdicional
                    {
                        IdUsuario = perfil.IdUsuario,
                        Telefono = perfil.Telefono,
                        Direccion = perfil.Direccion,
                        FechaNacimiento = perfil.FechaNacimiento,
                        FotoPath = perfil.FotoPath
                    };
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpPost]
        public IActionResult EditarAdicional(InformacionAdicional model, IFormFile foto)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            string fotoPath = model.FotoPath;

            // Manejar subida de foto
            if (foto != null && foto.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ViewBag.Error = "Solo se permiten archivos de imagen (jpg, jpeg, png, gif)";
                    return View(model);
                }

                if (foto.Length > 5 * 1024 * 1024) // 5MB
                {
                    ViewBag.Error = "El archivo no puede superar los 5MB";
                    return View(model);
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "profiles");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{model.IdUsuario}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    foto.CopyTo(fileStream);
                }

                fotoPath = $"/uploads/profiles/{uniqueFileName}";
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<dynamic>(
                    "ActualizarInformacionAdicional",
                    new
                    {
                        IdUsuario = model.IdUsuario,
                        Telefono = model.Telefono,
                        Direccion = model.Direccion,
                        FechaNacimiento = model.FechaNacimiento,
                        FotoPath = fotoPath
                    },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (resultado.Resultado > 0)
                {
                    ViewBag.Exito = "Información adicional actualizada exitosamente";
                    model.FotoPath = fotoPath;
                    return View(model);
                }
                else
                {
                    ViewBag.Error = "Error al actualizar la información";
                    return View(model);
                }
            }
        }
        #endregion

        #region Cambiar Contraseña
        [HttpGet]
        public IActionResult CambiarContrasena()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new CambiarContrasena
            {
                IdUsuario = Convert.ToInt64(HttpContext.Session.GetString("IdUsuario"))
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult CambiarContrasena(CambiarContrasena model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(model.ContrasenaActual) || string.IsNullOrEmpty(model.ContrasenaNueva) ||
                string.IsNullOrEmpty(model.ConfirmarContrasena))
            {
                ViewBag.Error = "Todos los campos son obligatorios";
                return View(model);
            }

            if (model.ContrasenaNueva != model.ConfirmarContrasena)
            {
                ViewBag.Error = "Las contraseñas no coinciden";
                return View(model);
            }

            if (model.ContrasenaNueva.Length < 6)
            {
                ViewBag.Error = "La contraseña debe tener al menos 6 caracteres";
                return View(model);
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                string contrasenaActualEncriptada = EncriptarContrasena(model.ContrasenaActual);
                string contrasenaNuevaEncriptada = EncriptarContrasena(model.ContrasenaNueva);

                var resultado = context.QueryFirstOrDefault<dynamic>(
                    "CambiarContrasena",
                    new
                    {
                        IdUsuario = model.IdUsuario,
                        ContrasenaActual = contrasenaActualEncriptada,
                        ContrasenaNueva = contrasenaNuevaEncriptada
                    },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (resultado.Resultado > 0)
                {
                    ViewBag.Exito = "Contraseña actualizada exitosamente";
                    return View(new CambiarContrasena { IdUsuario = model.IdUsuario });
                }
                else
                {
                    ViewBag.Error = resultado.Mensaje;
                    return View(model);
                }
            }
        }
        #endregion

        #region Métodos Privados
        private string EncriptarContrasena(string contrasena)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(contrasena);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
        #endregion
    }
}