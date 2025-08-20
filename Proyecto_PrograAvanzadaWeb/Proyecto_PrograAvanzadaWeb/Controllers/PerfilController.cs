using Proyecto_PrograAvanzadaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Proyecto_PrograAvanzadaWeb.Services;

namespace Proyecto_PrograAvanzadaWeb.Controllers
{
    public class PerfilController : Controller
    {
        private readonly ApiService _apiService;

        public PerfilController(ApiService apiService)
        {
            _apiService = apiService;
        }

        #region Ver Perfil
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var response = await _apiService.ObtenerPerfilCompleto();

                if (!response.Success || response.Data == null)
                {
                    ViewBag.Error = response.Message ?? "Error al cargar el perfil";
                    return View(new PerfilUsuarioCompleto());
                }

                // Mapear DTO a modelo de vista existente
                var perfil = new PerfilUsuarioCompleto
                {
                    IdUsuario = response.Data.IdUsuario,
                    Nombre = response.Data.Nombre,
                    Correo = response.Data.Correo,
                    Identificacion = response.Data.Identificacion,
                    Telefono = response.Data.Telefono,
                    Direccion = response.Data.Direccion,
                    FechaNacimiento = response.Data.FechaNacimiento,
                    FotoPath = response.Data.FotoPath,
                    Estado = response.Data.Estado,
                    IdRol = response.Data.IdRol,
                    NombreRol = response.Data.NombreRol,
                    FechaRegistro = response.Data.FechaRegistro,
                    FechaActualizacion = response.Data.FechaActualizacion
                };

                return View(perfil);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error de conexión: {ex.Message}";
                return View(new PerfilUsuarioCompleto());
            }
        }
        #endregion

        #region Editar Información Básica
        [HttpGet]
        public async Task<IActionResult> EditarBasico()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var response = await _apiService.ObtenerPerfilCompleto();

                if (!response.Success || response.Data == null)
                {
                    TempData["Error"] = response.Message ?? "Error al cargar el perfil";
                    return RedirectToAction("Index");
                }

                var model = new ActualizarPerfilBasicoModel
                {
                    IdUsuario = response.Data.IdUsuario,
                    Nombre = response.Data.Nombre,
                    Correo = response.Data.Correo,
                    Identificacion = response.Data.Identificacion
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error de conexión: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditarBasico(ActualizarPerfilBasicoModel model)
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

            try
            {
                var dto = new ActualizarPerfilBasicoDto
                {
                    Nombre = model.Nombre,
                    Correo = model.Correo,
                    Identificacion = model.Identificacion
                };

                var response = await _apiService.ActualizarPerfilBasico(dto);

                if (response.Success)
                {
                    // Actualizar datos de sesión
                    HttpContext.Session.SetString("Nombre", model.Nombre);
                    HttpContext.Session.SetString("Correo", model.Correo);

                    ViewBag.Exito = response.Message ?? "Perfil actualizado exitosamente";
                    return View(model);
                }
                else
                {
                    ViewBag.Error = response.Message ?? "Error al actualizar el perfil";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error de conexión: {ex.Message}";
                return View(model);
            }
        }
        #endregion

        #region Editar Información Adicional
        [HttpGet]
        public async Task<IActionResult> EditarAdicional()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var response = await _apiService.ObtenerPerfilCompleto();

                if (!response.Success || response.Data == null)
                {
                    TempData["Error"] = response.Message ?? "Error al cargar el perfil";
                    return RedirectToAction("Index");
                }

                var model = new InformacionAdicionalModel
                {
                    IdUsuario = response.Data.IdUsuario,
                    Telefono = response.Data.Telefono,
                    Direccion = response.Data.Direccion,
                    FechaNacimiento = response.Data.FechaNacimiento,
                    FotoPath = response.Data.FotoPath
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error de conexión: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditarAdicional(InformacionAdicionalModel model, IFormFile foto)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                string fotoPath = model.FotoPath;

                // Manejar subida de foto si se proporciona
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

                    // Subir foto a través de la API
                    var fotoResponse = await _apiService.SubirFotoPerfil(foto);

                    if (fotoResponse.Success && fotoResponse.Data != null)
                    {
                        fotoPath = fotoResponse.Data.FotoPath;
                        model.FotoPath = fotoPath;
                        ViewBag.Exito = "Foto de perfil actualizada exitosamente";
                        return View(model);
                    }
                    else
                    {
                        ViewBag.Error = fotoResponse.Message ?? "Error al subir la foto";
                        return View(model);
                    }
                }

                // Actualizar información adicional (sin foto)
                var dto = new ActualizarInformacionAdicionalDto
                {
                    Telefono = model.Telefono,
                    Direccion = model.Direccion,
                    FechaNacimiento = model.FechaNacimiento
                };

                var response = await _apiService.ActualizarInformacionAdicional(dto);

                if (response.Success)
                {
                    ViewBag.Exito = response.Message ?? "Información adicional actualizada exitosamente";
                    return View(model);
                }
                else
                {
                    ViewBag.Error = response.Message ?? "Error al actualizar la información";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error de conexión: {ex.Message}";
                return View(model);
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

            var model = new CambiarContrasenaModel
            {
                IdUsuario = Convert.ToInt64(HttpContext.Session.GetString("IdUsuario"))
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarContrasena(CambiarContrasenaModel model)
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

            try
            {
                var dto = new CambiarContrasenaPerfilDto
                {
                    ContrasenaActual = model.ContrasenaActual,
                    ContrasenaNueva = model.ContrasenaNueva
                };

                var response = await _apiService.CambiarContrasena(dto);

                if (response.Success)
                {
                    ViewBag.Exito = response.Message ?? "Contraseña actualizada exitosamente";
                    return View(new CambiarContrasenaModel { IdUsuario = model.IdUsuario });
                }
                else
                {
                    ViewBag.Error = response.Message ?? "Error al cambiar la contraseña";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error de conexión: {ex.Message}";
                return View(model);
            }
        }
        #endregion
    }
}