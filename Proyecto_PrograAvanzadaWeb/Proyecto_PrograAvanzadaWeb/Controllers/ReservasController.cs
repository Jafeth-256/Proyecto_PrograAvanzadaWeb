using Microsoft.AspNetCore.Mvc;
using Proyecto_PrograAvanzadaWeb.Services;
using Proyecto_PrograAvanzadaWeb.Models;

namespace Proyecto_PrograAvanzadaWeb.Controllers
{
    public class ReservasController : Controller
    {
        private readonly ApiService _apiService;

        public ReservasController(ApiService apiService)
        {
            _apiService = apiService;
        }

        #region Explorar Tours
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
                var response = await _apiService.ObtenerToursDisponibles();

                if (!response.Success)
                {
                    ViewBag.Error = response.Message;
                    return View(new List<TourDisponibleViewModel>());
                }

                if (response.Data == null || !response.Data.Any())
                {
                    ViewBag.Mensaje = "No hay tours disponibles en este momento.";
                    return View(new List<TourDisponibleViewModel>());
                }

                var tours = response.Data.Select(t => new TourDisponibleViewModel
                {
                    IdTour = t.IdTour,
                    Nombre = t.Nombre ?? "Tour sin nombre",
                    Descripcion = t.Descripcion ?? "Sin descripción disponible",
                    Destino = t.Destino ?? "Destino no especificado",
                    Precio = t.Precio,
                    FechaInicio = t.FechaInicio,
                    FechaFin = t.FechaFin,
                    CantidadPersonas = t.CantidadPersonas,
                    NombreCreador = t.NombreCreador ?? "Organizador desconocido",
                    PersonasReservadas = t.PersonasReservadas,
                    CuposDisponibles = t.CuposDisponibles
                }).ToList();

                return View(tours);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar los tours: {ex.Message}";
                return View(new List<TourDisponibleViewModel>());
            }
        }
        #endregion

        #region Detalle del Tour
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
                var response = await _apiService.ObtenerTourDisponiblePorId(id);

                if (!response.Success || response.Data == null)
                {
                    TempData["Error"] = response.Message ?? "Tour no encontrado o no disponible";
                    return RedirectToAction("Explorar");
                }

                var tour = new TourDisponibleViewModel
                {
                    IdTour = response.Data.IdTour,
                    Nombre = response.Data.Nombre ?? "Tour sin nombre",
                    Descripcion = response.Data.Descripcion ?? "Sin descripción disponible",
                    Destino = response.Data.Destino ?? "Destino no especificado",
                    Precio = response.Data.Precio,
                    FechaInicio = response.Data.FechaInicio,
                    FechaFin = response.Data.FechaFin,
                    CantidadPersonas = response.Data.CantidadPersonas,
                    NombreCreador = response.Data.NombreCreador ?? "Organizador desconocido",
                    PersonasReservadas = response.Data.PersonasReservadas,
                    CuposDisponibles = response.Data.CuposDisponibles
                };

                return View(tour);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el tour: {ex.Message}";
                return RedirectToAction("Explorar");
            }
        }
        #endregion

        #region Crear Reserva
        [HttpGet]
        public async Task<IActionResult> CrearReserva(long idTour)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");

            try
            {
                var response = await _apiService.ObtenerTourDisponiblePorId(idTour);

                if (!response.Success || response.Data == null)
                {
                    TempData["Error"] = response.Message ?? "Tour no encontrado o no disponible";
                    return RedirectToAction("Explorar");
                }

                var modelo = new CrearReservaViewModel
                {
                    IdTour = idTour,
                    CantidadPersonas = 1,
                    Tour = new TourDisponibleViewModel
                    {
                        IdTour = response.Data.IdTour,
                        Nombre = response.Data.Nombre ?? "Tour sin nombre",
                        Descripcion = response.Data.Descripcion ?? "Sin descripción disponible",
                        Destino = response.Data.Destino ?? "Destino no especificado",
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
        public async Task<IActionResult> CrearReserva(CrearReservaViewModel modelo)
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

                var dto = new CrearReservaDto
                {
                    IdTour = modelo.IdTour,
                    CantidadPersonas = modelo.CantidadPersonas,
                    Comentarios = modelo.Comentarios
                };

                var response = await _apiService.CrearReserva(dto);

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

        private async Task<IActionResult> RecargarFormularioReserva(CrearReservaViewModel modelo)
        {
            try
            {
                var tourResponse = await _apiService.ObtenerTourDisponiblePorId(modelo.IdTour);
                if (tourResponse.Success && tourResponse.Data != null)
                {
                    modelo.Tour = new TourDisponibleViewModel
                    {
                        IdTour = tourResponse.Data.IdTour,
                        Nombre = tourResponse.Data.Nombre ?? "Tour sin nombre",
                        Descripcion = tourResponse.Data.Descripcion ?? "Sin descripción disponible",
                        Destino = tourResponse.Data.Destino ?? "Destino no especificado",
                        Precio = tourResponse.Data.Precio,
                        FechaInicio = tourResponse.Data.FechaInicio,
                        FechaFin = tourResponse.Data.FechaFin,
                        CantidadPersonas = tourResponse.Data.CantidadPersonas,
                        NombreCreador = tourResponse.Data.NombreCreador ?? "Organizador desconocido",
                        PersonasReservadas = tourResponse.Data.PersonasReservadas,
                        CuposDisponibles = tourResponse.Data.CuposDisponibles
                    };
                }
                return View("CrearReserva", modelo);
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
                var response = await _apiService.ObtenerMisReservas();

                if (!response.Success)
                {
                    ViewBag.Error = response.Message;
                    return View(new List<ReservaViewModel>());
                }

                if (response.Data == null)
                {
                    ViewBag.Mensaje = "No tienes reservas aún.";
                    return View(new List<ReservaViewModel>());
                }

                var reservas = response.Data.Select(r => new ReservaViewModel
                {
                    IdReserva = r.IdReserva,
                    IdTour = r.IdTour,
                    NombreTour = r.NombreTour ?? "Tour sin nombre",
                    Destino = r.Destino ?? "Destino no especificado",
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
                return View(new List<ReservaViewModel>());
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
                var response = await _apiService.CancelarReserva(idReserva);

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