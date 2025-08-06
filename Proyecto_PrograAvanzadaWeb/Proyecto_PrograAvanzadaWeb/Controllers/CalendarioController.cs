
using Proyecto_PrograAvanzadaWeb.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Proyecto_PrograAvanzadaWeb.Controllers
{
    public class CalendarioController : Controller
    {
        private readonly IConfiguration _configuration;

        public CalendarioController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Vista de Calendario General
        public IActionResult Index()
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                // Obtener tours activos
                var tours = context.Query<dynamic>(
                    @"SELECT IdTour as Id, Nombre as Titulo, 'Tour' as Tipo, 
                      FechaInicio as FechaInicio, FechaFin as FechaFin, 
                      Destino as Ubicacion, Precio
                      FROM TTour 
                      WHERE Estado = 1 AND FechaFin >= GETDATE()",
                    commandType: System.Data.CommandType.Text
                ).ToList();

                // Obtener eventos activos
                var eventos = context.Query<dynamic>(
                    @"SELECT Id, Titulo, 'Evento' as Tipo, 
                      Fecha as FechaInicio, Fecha as FechaFin, 
                      Ubicacion, 0 as Precio
                      FROM Eventos 
                      WHERE Fecha >= GETDATE()",
                    commandType: System.Data.CommandType.Text
                ).ToList();

                // Combinar todos los items
                var calendarioItems = new List<dynamic>();
                calendarioItems.AddRange(tours);
                calendarioItems.AddRange(eventos);

                return View(calendarioItems);
            }
        }
        #endregion

        #region Calendario Personal
        public IActionResult MiCalendario()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            var idUsuario = long.Parse(HttpContext.Session.GetString("IdUsuario"));

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var items = context.Query<dynamic>(
                    "ConsultarCalendarioPersonal",
                    new { IdUsuario = idUsuario },
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();

                return View(items);
            }
        }
        #endregion

        #region Agregar/Quitar de Calendario Personal
        [HttpPost]
        public IActionResult AgregarACalendario(long itemId, string tipo)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return Json(new { success = false, message = "Debe iniciar sesión" });
            }

            var idUsuario = long.Parse(HttpContext.Session.GetString("IdUsuario"));

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<dynamic>(
                    "AgregarItemCalendario",
                    new
                    {
                        IdUsuario = idUsuario,
                        IdItem = itemId,
                        TipoItem = tipo
                    },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                return Json(new { success = resultado.Resultado > 0, message = resultado.Mensaje });
            }
        }

        [HttpPost]
        public IActionResult QuitarDeCalendario(long itemId, string tipo)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return Json(new { success = false, message = "Debe iniciar sesión" });
            }

            var idUsuario = long.Parse(HttpContext.Session.GetString("IdUsuario"));

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<dynamic>(
                    "QuitarItemCalendario",
                    new
                    {
                        IdUsuario = idUsuario,
                        IdItem = itemId,
                        TipoItem = tipo
                    },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                return Json(new { success = resultado.Resultado > 0, message = resultado.Mensaje });
            }
        }
        #endregion

        #region API para FullCalendar
        [HttpGet]
        public IActionResult ObtenerEventosCalendario(DateTime start, DateTime end)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var query = @"
                    SELECT IdTour as id, Nombre as title, FechaInicio as start, 
                           FechaFin as end, '#10b981' as color, 'tour' as tipo
                    FROM TTour 
                    WHERE Estado = 1 AND FechaInicio >= @Start AND FechaFin <= @End
                    
                    UNION ALL
                    
                    SELECT Id as id, Titulo as title, Fecha as start, 
                           Fecha as end, '#3b82f6' as color, 'evento' as tipo
                    FROM Eventos 
                    WHERE Fecha >= @Start AND Fecha <= @End";

                var eventos = context.Query<dynamic>(
                    query,
                    new { Start = start, End = end },
                    commandType: System.Data.CommandType.Text
                ).ToList();

                return Json(eventos);
            }
        }

        [HttpGet]
        public IActionResult ObtenerEventosPersonales(DateTime start, DateTime end)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return Json(new List<dynamic>());
            }

            var idUsuario = long.Parse(HttpContext.Session.GetString("IdUsuario"));

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var eventos = context.Query<dynamic>(
                    "ConsultarEventosCalendarioPersonal",
                    new { IdUsuario = idUsuario, FechaInicio = start, FechaFin = end },
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();

                return Json(eventos);
            }
        }
        #endregion
    }
}