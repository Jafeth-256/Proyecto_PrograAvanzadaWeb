using Proyecto_PrograAvanzadaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Proyecto_PrograAvanzadaWeb.Services;

namespace Proyecto_PrograAvanzadaWeb.Controllers
{
    public class ToursController : Controller
    {
        private readonly ApiService _apiService;

        public ToursController(ApiService apiService)
        {
            _apiService = apiService;
        }

        #region Listar Tours
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.ObtenerTodosLosTours();

                if (!response.Success || response.Data == null)
                {
                    ViewBag.Error = response.Message ?? "Error al cargar los tours";
                    return View(new List<TourViewModel>());
                }

                // Mapear DTOs a ViewModels
                var toursViewModel = response.Data.Select(t => new TourViewModel
                {
                    IdTour = t.IdTour,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    Destino = t.Destino,
                    Precio = t.Precio,
                    FechaInicio = t.FechaInicio,
                    FechaFin = t.FechaFin,
                    CantidadPersonas = t.CantidadPersonas,
                    NombreCreador = t.NombreCreador
                }).ToList();

                return View(toursViewModel);
            }
            catch (Exception ex)
            {
                // Log solo en development, no mostrar al usuario
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    ViewBag.Error = $"Error de desarrollo: {ex.Message}";
                }
                else
                {
                    ViewBag.Error = "Error al cargar los tours. Inténtalo de nuevo.";
                }
                return View(new List<TourViewModel>());
            }
        }
        #endregion

        #region Ver Detalle Tour
        [HttpGet]
        public async Task<IActionResult> Detalle(long id)
        {
            try
            {
                var response = await _apiService.ObtenerTourPorId(id);

                if (!response.Success || response.Data == null)
                {
                    TempData["Error"] = response.Message ?? "Tour no encontrado";
                    return RedirectToAction("Index");
                }

                var tourViewModel = new TourViewModel
                {
                    IdTour = response.Data.IdTour,
                    Nombre = response.Data.Nombre,
                    Descripcion = response.Data.Descripcion,
                    Destino = response.Data.Destino,
                    Precio = response.Data.Precio,
                    FechaInicio = response.Data.FechaInicio,
                    FechaFin = response.Data.FechaFin,
                    CantidadPersonas = response.Data.CantidadPersonas,
                    NombreCreador = response.Data.NombreCreador
                };

                return View(tourViewModel);
            }
            catch (Exception)
            {
                TempData["Error"] = "Error al cargar los detalles del tour";
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Crear Tour
        [HttpGet]
        public IActionResult CrearTour()
        {
            // Verificar que el usuario esté logueado
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("NombreRol");
            if (rol != "Usuario Administrador")
            {
                TempData["Error"] = "No tienes permisos para crear tours";
                return RedirectToAction("Index");
            }

            return View(new CrearTourViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CrearTour(CrearTourViewModel model)
        {
            // Verificar que el usuario esté logueado
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("NombreRol");
            if (rol != "Usuario Administrador")
            {
                TempData["Error"] = "No tienes permisos para crear tours";
                return RedirectToAction("Index");
            }

            // Validaciones básicas
            if (string.IsNullOrEmpty(model.Nombre) || string.IsNullOrEmpty(model.Descripcion) ||
                string.IsNullOrEmpty(model.Destino) || model.Precio <= 0 || model.CantidadPersonas <= 0)
            {
                ViewBag.Error = "Todos los campos son obligatorios y deben tener valores válidos";
                return View(model);
            }

            if (model.FechaInicio <= DateTime.Now.Date)
            {
                ViewBag.Error = "La fecha de inicio debe ser futura";
                return View(model);
            }

            if (model.FechaFin <= model.FechaInicio)
            {
                ViewBag.Error = "La fecha de fin debe ser posterior a la fecha de inicio";
                return View(model);
            }

            try
            {
                var dto = new CrearTourDto
                {
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    Destino = model.Destino,
                    Precio = model.Precio,
                    FechaInicio = model.FechaInicio,
                    FechaFin = model.FechaFin,
                    CantidadPersonas = model.CantidadPersonas
                };

                var response = await _apiService.CrearTour(dto);

                if (response.Success)
                {
                    TempData["Exito"] = response.Message ?? "Tour creado exitosamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = response.Message ?? "Error al crear el tour";
                    return View(model);
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Error al procesar la solicitud. Inténtalo de nuevo.";
                return View(model);
            }
        }
        #endregion

        #region Editar Tour
        [HttpGet]
        public async Task<IActionResult> EditarTour(long id)
        {
            // Verificar que el usuario esté logueado
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("NombreRol");
            if (rol != "Usuario Administrador")
            {
                TempData["Error"] = "No tienes permisos para editar tours";
                return RedirectToAction("Index");
            }

            try
            {
                var response = await _apiService.ObtenerTourPorId(id);

                if (!response.Success || response.Data == null)
                {
                    TempData["Error"] = response.Message ?? "Tour no encontrado";
                    return RedirectToAction("Index");
                }

                var model = new EditarTourViewModel
                {
                    IdTour = response.Data.IdTour,
                    Nombre = response.Data.Nombre,
                    Descripcion = response.Data.Descripcion,
                    Destino = response.Data.Destino,
                    Precio = response.Data.Precio,
                    FechaInicio = response.Data.FechaInicio,
                    FechaFin = response.Data.FechaFin,
                    CantidadPersonas = response.Data.CantidadPersonas
                };

                return View(model);
            }
            catch (Exception)
            {
                TempData["Error"] = "Error al cargar el tour para editar";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditarTour(EditarTourViewModel model)
        {
            // Verificar que el usuario esté logueado
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("NombreRol");
            if (rol != "Usuario Administrador")
            {
                TempData["Error"] = "No tienes permisos para editar tours";
                return RedirectToAction("Index");
            }

            // Validaciones básicas
            if (string.IsNullOrEmpty(model.Nombre) || string.IsNullOrEmpty(model.Descripcion) ||
                string.IsNullOrEmpty(model.Destino) || model.Precio <= 0 || model.CantidadPersonas <= 0)
            {
                ViewBag.Error = "Todos los campos son obligatorios y deben tener valores válidos";
                return View(model);
            }

            if (model.FechaFin <= model.FechaInicio)
            {
                ViewBag.Error = "La fecha de fin debe ser posterior a la fecha de inicio";
                return View(model);
            }

            try
            {
                var dto = new CrearTourDto
                {
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    Destino = model.Destino,
                    Precio = model.Precio,
                    FechaInicio = model.FechaInicio,
                    FechaFin = model.FechaFin,
                    CantidadPersonas = model.CantidadPersonas
                };

                var response = await _apiService.ActualizarTour(model.IdTour, dto);

                if (response.Success)
                {
                    TempData["Exito"] = response.Message ?? "Tour actualizado exitosamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = response.Message ?? "Error al actualizar el tour";
                    return View(model);
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Error al procesar la solicitud. Inténtalo de nuevo.";
                return View(model);
            }
        }
        #endregion

        #region Eliminar Tour
        [HttpPost]
        public async Task<IActionResult> Eliminar(long id)
        {
            // Verificar que el usuario esté logueado
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("NombreRol");
            if (rol != "Usuario Administrador")
            {
                TempData["Error"] = "No tienes permisos para eliminar tours";
                return RedirectToAction("Index");
            }

            try
            {
                var response = await _apiService.EliminarTour(id);

                if (response.Success)
                {
                    TempData["Exito"] = response.Message ?? "Tour eliminado exitosamente";
                }
                else
                {
                    TempData["Error"] = response.Message ?? "Error al eliminar el tour";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Error al procesar la solicitud de eliminación";
            }

            return RedirectToAction("Index");
        }
        #endregion
    }
}