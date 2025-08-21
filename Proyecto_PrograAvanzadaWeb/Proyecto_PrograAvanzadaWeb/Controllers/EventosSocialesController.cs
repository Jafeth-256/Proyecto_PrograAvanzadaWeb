using Microsoft.AspNetCore.Mvc;
using Proyecto_PrograAvanzadaWeb.Services;
using Proyecto_PrograAvanzadaWeb.Models;

namespace Proyecto_PrograAvanzadaWeb.Controllers
{
    public class EventosSocialesController : Controller
    {
        private readonly ApiService _apiService;

        public EventosSocialesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        #region Gestión de Eventos Sociales (Admin)
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            try
            {
                var response = await _apiService.ObtenerTodosLosEventosSociales();

                if (!response.Success)
                {
                    ViewBag.Error = response.Message;
                    return View(new List<EventoSocialViewModel>());
                }

                var eventos = response.Data?.Select(e => new EventoSocialViewModel
                {
                    IdEventoSocial = e.IdEventoSocial,
                    Nombre = e.Nombre ?? "Evento sin nombre",
                    Descripcion = e.Descripcion ?? "Sin descripción",
                    Ubicacion = e.Ubicacion ?? "Ubicación no especificada",
                    Precio = e.Precio,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    CantidadPersonas = e.CantidadPersonas,
                    NombreCreador = e.NombreCreador ?? "Organizador desconocido"
                }).ToList() ?? new List<EventoSocialViewModel>();

                return View(eventos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar eventos sociales: {ex.Message}";
                return View(new List<EventoSocialViewModel>());
            }
        }

        [HttpGet]
        public IActionResult CrearEvento()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            if (HttpContext.Session.GetString("NombreRol") != "Usuario Administrador")
            {
                return RedirectToAction("Index");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            return View(new CrearEventoSocialViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CrearEvento(CrearEventoSocialViewModel modelo)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            if (HttpContext.Session.GetString("NombreRol") != "Usuario Administrador")
            {
                return RedirectToAction("Index");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                var dto = new CrearEventoSocialDto
                {
                    Nombre = modelo.Nombre,
                    Descripcion = modelo.Descripcion,
                    Ubicacion = modelo.Ubicacion,
                    Precio = modelo.Precio,
                    FechaInicio = modelo.FechaInicio,
                    FechaFin = modelo.FechaFin,
                    CantidadPersonas = modelo.CantidadPersonas
                };

                var response = await _apiService.CrearEventoSocial(dto);

                if (response.Success)
                {
                    TempData["Exito"] = "Evento social creado exitosamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = response.Message;
                    return View(modelo);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al crear evento social: {ex.Message}";
                return View(modelo);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditarEvento(long id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            if (HttpContext.Session.GetString("NombreRol") != "Usuario Administrador")
            {
                return RedirectToAction("Index");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            try
            {
                var response = await _apiService.ObtenerEventoSocialPorId(id);

                if (!response.Success || response.Data == null)
                {
                    TempData["Error"] = response.Message ?? "Evento no encontrado";
                    return RedirectToAction("Index");
                }

                var modelo = new CrearEventoSocialViewModel
                {
                    Nombre = response.Data.Nombre,
                    Descripcion = response.Data.Descripcion,
                    Ubicacion = response.Data.Ubicacion,
                    Precio = response.Data.Precio,
                    FechaInicio = response.Data.FechaInicio,
                    FechaFin = response.Data.FechaFin,
                    CantidadPersonas = response.Data.CantidadPersonas
                };

                ViewData["IdEvento"] = id;
                return View(modelo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar evento: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditarEvento(long idEvento, CrearEventoSocialViewModel modelo)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            if (HttpContext.Session.GetString("NombreRol") != "Usuario Administrador")
            {
                return RedirectToAction("Index");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            if (!ModelState.IsValid)
            {
                ViewData["IdEvento"] = idEvento;
                return View(modelo);
            }

            try
            {
                var dto = new CrearEventoSocialDto
                {
                    Nombre = modelo.Nombre,
                    Descripcion = modelo.Descripcion,
                    Ubicacion = modelo.Ubicacion,
                    Precio = modelo.Precio,
                    FechaInicio = modelo.FechaInicio,
                    FechaFin = modelo.FechaFin,
                    CantidadPersonas = modelo.CantidadPersonas
                };

                var response = await _apiService.ActualizarEventoSocial(idEvento, dto);

                if (response.Success)
                {
                    TempData["Exito"] = "Evento social actualizado exitosamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = response.Message;
                    ViewData["IdEvento"] = idEvento;
                    return View(modelo);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al actualizar evento social: {ex.Message}";
                ViewData["IdEvento"] = idEvento;
                return View(modelo);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(long id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            if (HttpContext.Session.GetString("NombreRol") != "Usuario Administrador")
            {
                return RedirectToAction("Index");
            }

            try
            {
                var response = await _apiService.EliminarEventoSocial(id);

                if (response.Success)
                {
                    TempData["Exito"] = "Evento social eliminado exitosamente";
                }
                else
                {
                    TempData["Error"] = response.Message;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar evento social: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detalle(long id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            try
            {
                var response = await _apiService.ObtenerEventoSocialPorId(id);

                if (!response.Success || response.Data == null)
                {
                    TempData["Error"] = response.Message ?? "Evento no encontrado";
                    return RedirectToAction("Index");
                }

                var evento = new EventoSocialViewModel
                {
                    IdEventoSocial = response.Data.IdEventoSocial,
                    Nombre = response.Data.Nombre ?? "Evento sin nombre",
                    Descripcion = response.Data.Descripcion ?? "Sin descripción",
                    Ubicacion = response.Data.Ubicacion ?? "Ubicación no especificada",
                    Precio = response.Data.Precio,
                    FechaInicio = response.Data.FechaInicio,
                    FechaFin = response.Data.FechaFin,
                    CantidadPersonas = response.Data.CantidadPersonas,
                    NombreCreador = response.Data.NombreCreador ?? "Organizador desconocido"
                };

                return View(evento);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar evento: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Vista Pública
        public async Task<IActionResult> Sociales()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            try
            {
                var response = await _apiService.ObtenerTodosLosEventosSociales();

                if (!response.Success)
                {
                    ViewBag.Error = response.Message;
                    return View(new List<EventoSocialViewModel>());
                }

                // Filtrar solo eventos futuros para la vista pública
                var eventos = response.Data?.Where(e => e.FechaInicio > DateTime.Now)
                                          .Select(e => new EventoSocialViewModel
                                          {
                                              IdEventoSocial = e.IdEventoSocial,
                                              Nombre = e.Nombre ?? "Evento sin nombre",
                                              Descripcion = e.Descripcion ?? "Sin descripción",
                                              Ubicacion = e.Ubicacion ?? "Ubicación no especificada",
                                              Precio = e.Precio,
                                              FechaInicio = e.FechaInicio,
                                              FechaFin = e.FechaFin,
                                              CantidadPersonas = e.CantidadPersonas,
                                              NombreCreador = e.NombreCreador ?? "Organizador desconocido"
                                          }).OrderBy(e => e.FechaInicio)
                  .ToList() ?? new List<EventoSocialViewModel>();

                return View(eventos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar eventos sociales: {ex.Message}";
                return View(new List<EventoSocialViewModel>());
            }
        }
        #endregion
    }
}