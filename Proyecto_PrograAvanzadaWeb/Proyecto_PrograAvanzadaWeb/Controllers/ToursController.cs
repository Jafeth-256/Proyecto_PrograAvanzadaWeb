using Proyecto_PrograAvanzadaWeb.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Proyecto_PrograAvanzadaWeb.Controllers
{
    public class ToursController : Controller
    {
        private readonly IConfiguration _configuration;

        public ToursController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Listar Tours
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var tours = context.Query<Tour>("ConsultarTours", commandType: System.Data.CommandType.StoredProcedure).ToList();
                return View(tours);
            }
        }
        #endregion

        #region Crear Tour
        [HttpGet]
        public IActionResult CrearTour()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult CrearTour(string nombre, string descripcion, string destino, decimal precio,
                                 DateTime fechaInicio, DateTime fechaFin, int cantidadPersonas)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(descripcion) ||
                string.IsNullOrEmpty(destino) || precio <= 0 || cantidadPersonas <= 0)
            {
                ViewBag.Error = "Todos los campos son obligatorios";
                return View();
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var idUsuario = long.Parse(HttpContext.Session.GetString("IdUsuario"));

                var resultado = context.QueryFirstOrDefault<dynamic>(
                    "RegistrarTour",
                    new
                    {
                        Nombre = nombre,
                        Descripcion = descripcion,
                        Destino = destino,
                        Precio = precio,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin,
                        CantidadPersonas = cantidadPersonas,
                        IdUsuarioCreador = idUsuario
                    },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (resultado.Resultado > 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = resultado.Mensaje;
                    return View();
                }
            }
        }
        #endregion

        #region Editar Tour
        [HttpGet]
        public IActionResult EditarTour(long id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var tour = context.QueryFirstOrDefault<Tour>(
                    "ConsultarTourPorId",
                    new { IdTour = id },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (tour == null)
                {
                    return RedirectToAction("Index");
                }

                return View(tour);
            }
        }

        [HttpPost]
        public IActionResult EditarTour(long idTour, string nombre, string descripcion, string destino,
                                  decimal precio, DateTime fechaInicio, DateTime fechaFin, int cantidadPersonas)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(descripcion) ||
                string.IsNullOrEmpty(destino) || precio <= 0 || cantidadPersonas <= 0)
            {
                ViewBag.Error = "Todos los campos son obligatorios";

                using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
                {
                    var tour = context.QueryFirstOrDefault<Tour>(
                        "ConsultarTourPorId",
                        new { IdTour = idTour },
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                    return View(tour);
                }
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<dynamic>(
                    "ActualizarTour",
                    new
                    {
                        IdTour = idTour,
                        Nombre = nombre,
                        Descripcion = descripcion,
                        Destino = destino,
                        Precio = precio,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin,
                        CantidadPersonas = cantidadPersonas
                    },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (resultado.Resultado > 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = resultado.Mensaje;

                    var tour = context.QueryFirstOrDefault<Tour>(
                        "ConsultarTourPorId",
                        new { IdTour = idTour },
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                    return View(tour);
                }
            }
        }
        #endregion

        #region Eliminar Tour
        [HttpPost]
        public IActionResult Eliminar(long id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<dynamic>(
                    "EliminarTour",
                    new { IdTour = id },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                return RedirectToAction("Index");
            }
        }
        #endregion
    }
}