using Microsoft.AspNetCore.Mvc;
using Proyecto_PrograAvanzadaWeb.Services;
using Proyecto_PrograAvanzadaWeb.Models;

namespace Proyecto_PrograAvanzadaWeb.Controllers
{
    public class ReservasSocialesController : Controller
    {
        private readonly ApiService _apiService;

        public ReservasSocialesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        #region Explorar Eventos Sociales
        public async Task<IActionResult> Explorar()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            try
            {
                var response = await _apiService.ObtenerEventosSocialesDisponibles();

                if (!response.Success)
                {
                    ViewBag.Error = response.Message;
                    return View(new List<EventoSocialDisponibleViewModel>());
                }

                if (response.Data == null || !response.Data.Any())
                {
                    ViewBag.Mensaje = "No hay eventos sociales disponibles en este momento.";
                    return View(new List<EventoSocialDisponibleViewModel>());
                }

                var eventos = response.Data.Select(e => new EventoSocialDisponibleViewModel
                {
                    IdEventoSocial = e.IdEventoSocial,
                    Nombre = e.Nombre ?? "Evento sin nombre",
                    Descripcion = e.Descripcion ?? "Sin descripción disponible",
                    Ubicacion = e.Ubicacion ?? "Ubicación no especificada",
                    Precio = e.Precio,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    CantidadPersonas = e.CantidadPersonas,
                    NombreCreador = e.NombreCreador ?? "Organizador desconocido",
                    PersonasReservadas = e.PersonasReservadas,
                    CuposDisponibles = e.CuposDisponibles
                }).ToList();

                return View(eventos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar los eventos sociales: {ex.Message}";
                return View(new List<EventoSocialDisponibleViewModel>());
            }
        }
        #endregion

        #region Detalle del Evento Social
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
                var response = await _apiService.ObtenerEventoSocialDisponiblePorId(id);

                if (!response.Success || response.Data == null)
                {
                    TempData["Error"] = response.Message ?? "Evento social no encontrado o no disponible";
                    return RedirectToAction("Explorar");
                }

                var evento = new EventoSocialDisponibleViewModel
                {
                    IdEventoSocial = response.Data.IdEventoSocial,
                    Nombre = response.Data.Nombre ?? "Evento sin nombre",
                    Descripcion = response.Data.Descripcion ?? "Sin descripción disponible",
                    Ubicacion = response.Data.Ubicacion ?? "Ubicación no especificada",
                    Precio = response.Data.Precio,
                    FechaInicio = response.Data.FechaInicio,
                    FechaFin = response.Data.FechaFin,
                    CantidadPersonas = response.Data.CantidadPersonas,
                    NombreCreador = response.Data.NombreCreador ?? "Organizador desconocido",
                    PersonasReservadas = response.Data.PersonasReservadas,
                    CuposDisponibles = response.Data.CuposDisponibles
                };

                return View(evento);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el evento social: {ex.Message}";
                return RedirectToAction("Explorar");
            }
        }
        #endregion

        #region Crear Reserva
        [HttpGet]
        public async Task<IActionResult> CrearReserva(long idEvento)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            try
            {
                var response = await _apiService.ObtenerEventoSocialDisponiblePorId(idEvento);

                if (!response.Success || response.Data == null)
                {
                    TempData["Error"] = response.Message ?? "Evento social no encontrado o no disponible";
                    return RedirectToAction("Explorar");
                }

                var modelo = new CrearReservaSocialViewModel
                {
                    IdEventoSocial = idEvento,
                    CantidadPersonas = 1,
                    Evento = new EventoSocialDisponibleViewModel
                    {
                        IdEventoSocial = response.Data.IdEventoSocial,
                        Nombre = response.Data.Nombre ?? "Evento sin nombre",
                        Descripcion = response.Data.Descripcion ?? "Sin descripción disponible",
                        Ubicacion = response.Data.Ubicacion ?? "Ubicación no especificada",
                        Precio = response.Data.Precio,
                        FechaInicio = response.Data.FechaInicio,
                        FechaFin = response.Data.FechaFin,
                        CantidadPersonas = response.Data.CantidadPersonas,
                        NombreCreador = response.Data.NombreCreador ?? "Organizador desconocido",
                        PersonasReservadas = response.Data.PersonasReservadas,
                        CuposDisponibles = response.Data.CuposDisponibles
                    }
                };

                return View(modelo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el formulario de reserva: {ex.Message}";
                return RedirectToAction("Explorar");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearReserva(CrearReservaSocialViewModel modelo)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            try
            {
                if (modelo.CantidadPersonas <= 0)
                {
                    ViewBag.Error = "La cantidad de personas debe ser mayor a cero";
                    return await RecargarFormularioReserva(modelo);
                }

                var dto = new CrearReservaSocialDto
                {
                    IdEventoSocial = modelo.IdEventoSocial,
                    CantidadPersonas = modelo.CantidadPersonas,
                    Comentarios = modelo.Comentarios
                };

                var response = await _apiService.CrearReservaSocial(dto);

                if (response.Success)
                {
                    TempData["Exito"] = "Reserva creada exitosamente";
                    return RedirectToAction("MisReservas");
                }
                else
                {
                    ViewBag.Error = response.Message;
                    return await RecargarFormularioReserva(modelo);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al crear la reserva: {ex.Message}";
                return await RecargarFormularioReserva(modelo);
            }
        }

        private async Task<IActionResult> RecargarFormularioReserva(CrearReservaSocialViewModel modelo)
        {
            try
            {
                var eventoResponse = await _apiService.ObtenerEventoSocialDisponiblePorId(modelo.IdEventoSocial);
                if (eventoResponse.Success && eventoResponse.Data != null)
                {
                    modelo.Evento = new EventoSocialDisponibleViewModel
                    {
                        IdEventoSocial = eventoResponse.Data.IdEventoSocial,
                        Nombre = eventoResponse.Data.Nombre ?? "Evento sin nombre",
                        Descripcion = eventoResponse.Data.Descripcion ?? "Sin descripción disponible",
                        Ubicacion = eventoResponse.Data.Ubicacion ?? "Ubicación no especificada",
                        Precio = eventoResponse.Data.Precio,
                        FechaInicio = eventoResponse.Data.FechaInicio,
                        FechaFin = eventoResponse.Data.FechaFin,
                        CantidadPersonas = eventoResponse.Data.CantidadPersonas,
                        NombreCreador = eventoResponse.Data.NombreCreador ?? "Organizador desconocido",
                        PersonasReservadas = eventoResponse.Data.PersonasReservadas,
                        CuposDisponibles = eventoResponse.Data.CuposDisponibles
                    };
                }
                return View("~/Views/ReservasSociales/CrearReserva.cshtml", modelo);
            }
            catch (Exception)
            {
                return RedirectToAction("Explorar");
            }
        }
        #endregion

        #region Mis Reservas
        public async Task<IActionResult> MisReservas()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            try
            {
                var response = await _apiService.ObtenerMisReservasSociales();

                if (!response.Success)
                {
                    ViewBag.Error = response.Message;
                    return View(new List<ReservaSocialViewModel>());
                }

                if (response.Data == null)
                {
                    ViewBag.Mensaje = "No tienes reservas de eventos sociales aún.";
                    return View(new List<ReservaSocialViewModel>());
                }

                var reservas = response.Data.Select(r => new ReservaSocialViewModel
                {
                    IdReservaSocial = r.IdReservaSocial,
                    IdEventoSocial = r.IdEventoSocial,
                    NombreEvento = r.NombreEvento ?? "Evento sin nombre",
                    Ubicacion = r.Ubicacion ?? "Ubicación no especificada",
                    FechaInicio = r.FechaInicio,
                    FechaFin = r.FechaFin,
                    CantidadPersonas = r.CantidadPersonas,
                    PrecioTotal = r.PrecioTotal,
                    FechaReserva = r.FechaReserva,
                    EstadoReserva = r.EstadoReserva ?? "Pendiente",
                    Comentarios = r.Comentarios,
                    NombreCreador = r.NombreCreador ?? "Organizador desconocido"
                }).ToList();

                return View(reservas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar las reservas: {ex.Message}";
                return View(new List<ReservaSocialViewModel>());
            }
        }
        #endregion

        #region Cancelar Reserva
        [HttpPost]
        public async Task<IActionResult> CancelarReserva(long idReserva)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return Json(new { success = false, message = "Sesión expirada" });
            }

            try
            {
                var response = await _apiService.CancelarReservaSocial(idReserva);

                return Json(new { success = response.Success, message = response.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al cancelar la reserva: {ex.Message}" });
            }
        }
        #endregion
    }
}
