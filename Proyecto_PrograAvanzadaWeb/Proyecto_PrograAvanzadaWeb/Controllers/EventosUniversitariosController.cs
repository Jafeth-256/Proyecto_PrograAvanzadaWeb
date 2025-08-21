using Microsoft.AspNetCore.Mvc;
using Proyecto_PrograAvanzadaWeb.Services;
using Proyecto_PrograAvanzadaWeb.Models;

namespace Proyecto_PrograAvanzadaWeb.Controllers
{
    public class EventosUniversitariosController : Controller
    {
        private readonly ApiService _apiService;

        public EventosUniversitariosController(ApiService apiService)
        {
            _apiService = apiService;
        }

        #region Gestión de Eventos Universitarios
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
                var response = await _apiService.ObtenerTodosLosEventosUniversitarios();

                if (!response.Success)
                {
                    ViewBag.Error = response.Message;
                    return View(new List<EventoUniversitarioViewModel>());
                }

                var eventos = response.Data?.Select(e => new EventoUniversitarioViewModel
                {
                    IdEventoUniversitario = e.IdEventoUniversitario,
                    Nombre = e.Nombre ?? "Evento sin nombre",
                    Descripcion = e.Descripcion ?? "Sin descripción",
                    Ubicacion = e.Ubicacion ?? "Ubicación no especificada",
                    Universidad = e.Universidad ?? "Universidad no especificada",
                    Precio = e.Precio,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    CantidadPersonas = e.CantidadPersonas,
                    NombreCreador = e.NombreCreador ?? "Organizador desconocido"
                }).ToList() ?? new List<EventoUniversitarioViewModel>();

                return View(eventos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar eventos universitarios: {ex.Message}";
                return View(new List<EventoUniversitarioViewModel>());
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

            return View(new CrearEventoUniversitarioViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CrearEvento(CrearEventoUniversitarioViewModel modelo)
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
                var dto = new CrearEventoUniversitarioDto
                {
                    Nombre = modelo.Nombre,
                    Descripcion = modelo.Descripcion,
                    Ubicacion = modelo.Ubicacion,
                    Universidad = modelo.Universidad,
                    Precio = modelo.Precio,
                    FechaInicio = modelo.FechaInicio,
                    FechaFin = modelo.FechaFin,
                    CantidadPersonas = modelo.CantidadPersonas
                };

                var response = await _apiService.CrearEventoUniversitario(dto);

                if (response.Success)
                {
                    TempData["Exito"] = "Evento universitario creado exitosamente";
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
                ViewBag.Error = $"Error al crear evento universitario: {ex.Message}";
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
                var response = await _apiService.ObtenerEventoUniversitarioPorId(id);

                if (!response.Success || response.Data == null)
                {
                    TempData["Error"] = response.Message ?? "Evento no encontrado";
                    return RedirectToAction("Index");
                }

                var modelo = new CrearEventoUniversitarioViewModel
                {
                    Nombre = response.Data.Nombre,
                    Descripcion = response.Data.Descripcion,
                    Ubicacion = response.Data.Ubicacion,
                    Universidad = response.Data.Universidad,
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
        public async Task<IActionResult> EditarEvento(long idEvento, CrearEventoUniversitarioViewModel modelo)
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
                var dto = new CrearEventoUniversitarioDto
                {
                    Nombre = modelo.Nombre,
                    Descripcion = modelo.Descripcion,
                    Ubicacion = modelo.Ubicacion,
                    Universidad = modelo.Universidad,
                    Precio = modelo.Precio,
                    FechaInicio = modelo.FechaInicio,
                    FechaFin = modelo.FechaFin,
                    CantidadPersonas = modelo.CantidadPersonas
                };

                var response = await _apiService.ActualizarEventoUniversitario(idEvento, dto);

                if (response.Success)
                {
                    TempData["Exito"] = "Evento universitario actualizado exitosamente";
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
                ViewBag.Error = $"Error al actualizar evento universitario: {ex.Message}";
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
                var response = await _apiService.EliminarEventoUniversitario(id);

                if (response.Success)
                {
                    TempData["Exito"] = "Evento universitario eliminado exitosamente";
                }
                else
                {
                    TempData["Error"] = response.Message;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar evento universitario: {ex.Message}";
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
                var response = await _apiService.ObtenerEventoUniversitarioPorId(id);

                if (!response.Success || response.Data == null)
                {
                    TempData["Error"] = response.Message ?? "Evento no encontrado";
                    return RedirectToAction("Index");
                }

                var evento = new EventoUniversitarioViewModel
                {
                    IdEventoUniversitario = response.Data.IdEventoUniversitario,
                    Nombre = response.Data.Nombre ?? "Evento sin nombre",
                    Descripcion = response.Data.Descripcion ?? "Sin descripción",
                    Ubicacion = response.Data.Ubicacion ?? "Ubicación no especificada",
                    Universidad = response.Data.Universidad ?? "Universidad no especificada",
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
        public async Task<IActionResult> Universitarios()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            try
            {
                var response = await _apiService.ObtenerTodosLosEventosUniversitarios();

                if (!response.Success)
                {
                    ViewBag.Error = response.Message;
                    return View(new List<EventoUniversitarioViewModel>());
                }

                var eventos = response.Data?.Where(e => e.FechaInicio > DateTime.Now)
                                          .Select(e => new EventoUniversitarioViewModel
                                          {
                                              IdEventoUniversitario = e.IdEventoUniversitario,
                                              Nombre = e.Nombre ?? "Evento sin nombre",
                                              Descripcion = e.Descripcion ?? "Sin descripción",
                                              Ubicacion = e.Ubicacion ?? "Ubicación no especificada",
                                              Universidad = e.Universidad ?? "Universidad no especificada",
                                              Precio = e.Precio,
                                              FechaInicio = e.FechaInicio,
                                              FechaFin = e.FechaFin,
                                              CantidadPersonas = e.CantidadPersonas,
                                              NombreCreador = e.NombreCreador ?? "Organizador desconocido"
                                          }).ToList() ?? new List<EventoUniversitarioViewModel>();

                return View(eventos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar eventos universitarios: {ex.Message}";
                return View(new List<EventoUniversitarioViewModel>());
            }
        }
        #endregion
    }
}