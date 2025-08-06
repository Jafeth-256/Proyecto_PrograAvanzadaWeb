using Proyecto_PrograAvanzadaWeb.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace Proyecto_PrograAvanzadaWeb.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IConfiguration _configuration;

        public ReservasController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Ver Tours Disponibles (Galería)
        public IActionResult Tours()
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var tours = context.Query<Tour>(
                    "SELECT * FROM TTour WHERE Estado = 1 AND FechaFin >= GETDATE() ORDER BY FechaInicio",
                    commandType: System.Data.CommandType.Text
                ).ToList();

                ViewBag.Provincias = new List<string> { "San José", "Alajuela", "Cartago", "Heredia", "Guanacaste", "Puntarenas", "Limón" };
                ViewBag.TiposTour = new List<string> { "Aventura", "Playa", "Montaña", "Cultural", "Naturaleza", "Relax" };

                return View(tours);
            }
        }
        #endregion

        #region Buscar Tours con Filtros
        [HttpPost]
        public IActionResult BuscarTours(string provincia, string tipo, decimal? precioMin, decimal? precioMax, int? duracion)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var query = "SELECT * FROM TTour WHERE Estado = 1 AND FechaFin >= GETDATE()";
                var parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(provincia))
                {
                    query += " AND Destino LIKE @Provincia";
                    parameters.Add("Provincia", $"%{provincia}%");
                }

                if (!string.IsNullOrEmpty(tipo))
                {
                    query += " AND Descripcion LIKE @Tipo";
                    parameters.Add("Tipo", $"%{tipo}%");
                }

                if (precioMin.HasValue)
                {
                    query += " AND Precio >= @PrecioMin";
                    parameters.Add("PrecioMin", precioMin.Value);
                }

                if (precioMax.HasValue)
                {
                    query += " AND Precio <= @PrecioMax";
                    parameters.Add("PrecioMax", precioMax.Value);
                }

                if (duracion.HasValue)
                {
                    query += " AND DATEDIFF(day, FechaInicio, FechaFin) = @Duracion";
                    parameters.Add("Duracion", duracion.Value);
                }

                query += " ORDER BY FechaInicio";

                var tours = context.Query<Tour>(query, parameters).ToList();
                return PartialView("_ToursGrid", tours);
            }
        }
        #endregion

        #region Ver Detalle de Tour
        public IActionResult DetalleTour(long id)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var tour = context.QueryFirstOrDefault<Tour>(
                    "ConsultarTourPorId",
                    new { IdTour = id },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (tour == null)
                    return NotFound();

                return View(tour);
            }
        }
        #endregion

        #region Crear Reserva
        [HttpGet]
        public IActionResult CrearReserva(long id)
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
                    return NotFound();

                var model = new CrearReservaViewModel
                {
                    IdTour = tour.IdTour,
                    NombreTour = tour.Nombre,
                    PrecioPorPersona = tour.Precio
                };

                return View(model);
            }
        }

        [HttpPost]
        public IActionResult CrearReserva(CrearReservaViewModel model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            var idUsuario = long.Parse(HttpContext.Session.GetString("IdUsuario"));
            var numeroReserva = GenerarNumeroReserva();

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<dynamic>(
                    "RegistrarReserva",
                    new
                    {
                        IdTour = model.IdTour,
                        IdUsuario = idUsuario,
                        FechaTour = model.FechaTour,
                        CantidadPersonas = model.CantidadPersonas,
                        PrecioTotal = model.PrecioPorPersona * model.CantidadPersonas,
                        NumeroReserva = numeroReserva
                    },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (resultado.Resultado > 0)
                {
                    // Enviar correo de confirmación
                    EnviarCorreoConfirmacion(HttpContext.Session.GetString("Correo"), numeroReserva, model);

                    TempData["Exito"] = $"Reserva confirmada con número: {numeroReserva}";
                    return RedirectToAction("MisReservas");
                }
                else
                {
                    ViewBag.Error = "Error al crear la reserva";
                    return View(model);
                }
            }
        }
        #endregion

        #region Mis Reservas
        public IActionResult MisReservas()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index", "Home");
            }

            var idUsuario = long.Parse(HttpContext.Session.GetString("IdUsuario"));

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var reservas = context.Query<Reserva>(
                    "ConsultarReservasPorUsuario",
                    new { IdUsuario = idUsuario },
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();

                return View(reservas);
            }
        }
        #endregion

        #region Métodos Privados
        private string GenerarNumeroReserva()
        {
            return $"RES{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        private void EnviarCorreoConfirmacion(string correo, string numeroReserva, CrearReservaViewModel reserva)
        {
            try
            {
                // Configurar el cliente SMTP (ejemplo con Gmail)
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential("tucorreo@gmail.com", "tupassword")
                };

                var mensaje = new MailMessage
                {
                    From = new MailAddress("tucorreo@gmail.com", "Tico Tours"),
                    Subject = $"Confirmación de Reserva - {numeroReserva}",
                    Body = $@"
                        <h2>¡Reserva Confirmada!</h2>
                        <p>Tu reserva ha sido confirmada exitosamente.</p>
                        <p><strong>Número de reserva:</strong> {numeroReserva}</p>
                        <p><strong>Tour:</strong> {reserva.NombreTour}</p>
                        <p><strong>Fecha:</strong> {reserva.FechaTour:dd/MM/yyyy}</p>
                        <p><strong>Personas:</strong> {reserva.CantidadPersonas}</p>
                        <p><strong>Total:</strong> ₡{reserva.PrecioTotal:N0}</p>
                        <br>
                        <p>¡Gracias por confiar en nosotros!</p>
                    ",
                    IsBodyHtml = true
                };

                mensaje.To.Add(correo);
                smtpClient.Send(mensaje);
            }
            catch (Exception ex)
            {
                // Log del error
                Console.WriteLine($"Error al enviar correo: {ex.Message}");
            }
        }
        #endregion
    }
}