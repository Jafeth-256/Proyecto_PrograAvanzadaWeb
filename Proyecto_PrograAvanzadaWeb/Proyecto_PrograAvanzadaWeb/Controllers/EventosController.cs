using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class EventosController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public EventosController(IWebHostEnvironment env,
        UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _env = env;
        _userManager = userManager;
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("Connection");
    }

    public async Task<IActionResult> Index()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            // Consulta directa a la tabla de eventos
            var eventos = await connection.QueryAsync<Evento>(
                "SELECT * FROM TEventos ORDER BY Fecha"
            );

            // Si el usuario está autenticado, cargar sus favoritos
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                var favoritos = await connection.QueryAsync<int>(
                    "SELECT IdEvento FROM TEventoFavorito WHERE IdUsuario = @IdUsuario",
                    new { IdUsuario = userId }
                );

                ViewBag.Favoritos = favoritos.ToList();
            }

            return View(eventos);
        }
    }

    public async Task<IActionResult> Detalle(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var evento = await connection.QueryFirstOrDefaultAsync<Evento>(
                "SELECT * FROM TEventos WHERE Id = @Id",
                new { Id = id }
            );

            if (evento == null) return NotFound();

            // Verificar si el usuario lo tiene como favorito
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                var esFavorito = await connection.QueryFirstOrDefaultAsync<bool>(
                    "SELECT CASE WHEN EXISTS(SELECT 1 FROM TEventoFavorito WHERE IdUsuario = @IdUsuario AND IdEvento = @IdEvento) THEN 1 ELSE 0 END",
                    new { IdUsuario = userId, IdEvento = id }
                );

                ViewBag.EsFavorito = esFavorito;
            }

            return View(evento);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> MarcarFavorito(int eventoId, bool meInteresa)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Json(new { success = false, message = "Debe iniciar sesión" });
        }

        var userId = HttpContext.Session.GetString("IdUsuario");
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "Usuario no válido" });
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "MarcarEventoFavorito",
                new
                {
                    IdUsuario = long.Parse(userId),
                    IdEvento = eventoId,
                    MeInteresa = meInteresa
                },
                commandType: CommandType.StoredProcedure
            );

            return Json(new { success = resultado.Resultado > 0, message = resultado.Mensaje });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> QuitarFavorito(int eventoId)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Json(new { success = false, message = "Debe iniciar sesión" });
        }

        var userId = HttpContext.Session.GetString("IdUsuario");
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "Usuario no válido" });
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "QuitarEventoFavorito",
                new
                {
                    IdUsuario = long.Parse(userId),
                    IdEvento = eventoId
                },
                commandType: CommandType.StoredProcedure
            );

            return Json(new { success = resultado.Resultado > 0, message = resultado.Mensaje });
        }
    }

    [Authorize]
    public async Task<IActionResult> MisFavoritos()
    {
        var userId = HttpContext.Session.GetString("IdUsuario");
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Index", "Home");
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            var favoritos = await connection.QueryAsync<dynamic>(
                "ConsultarEventosFavoritos",
                new { IdUsuario = long.Parse(userId) },
                commandType: CommandType.StoredProcedure
            );

            return View(favoritos.ToList());
        }
    }

    [Authorize(Roles = "Organizador")]
    public IActionResult Crear() => View();

    [HttpPost]
    [Authorize(Roles = "Organizador")]
    public async Task<IActionResult> Crear(Evento evento, IFormFile Imagen)
    {
        if (ModelState.IsValid)
        {
            if (Imagen != null)
            {
                string rutaCarpeta = Path.Combine(_env.WebRootPath, "imagenes_eventos");
                Directory.CreateDirectory(rutaCarpeta);
                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(Imagen.FileName);
                string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await Imagen.CopyToAsync(stream);
                }

                evento.ImagenRuta = "/imagenes_eventos/" + nombreArchivo;
            }

            evento.UsuarioId = _userManager.GetUserId(User);

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO TEventos (Titulo, Descripcion, Tipo, Ubicacion, Fecha, Organizador, Contacto, ImagenRuta, UsuarioId) 
                           VALUES (@Titulo, @Descripcion, @Tipo, @Ubicacion, @Fecha, @Organizador, @Contacto, @ImagenRuta, @UsuarioId)";

                await connection.ExecuteAsync(sql, evento);
            }

            return RedirectToAction(nameof(Index));
        }
        return View(evento);
    }

    [Authorize(Roles = "Organizador")]
    public async Task<IActionResult> Editar(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var evento = await connection.QueryFirstOrDefaultAsync<Evento>(
                "SELECT * FROM TEventos WHERE Id = @Id",
                new { Id = id }
            );

            if (evento == null) return NotFound();
            return View(evento);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Organizador")]
    public async Task<IActionResult> Editar(Evento evento)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var sql = @"UPDATE TEventos 
                       SET Titulo = @Titulo, 
                           Descripcion = @Descripcion, 
                           Tipo = @Tipo,
                           Ubicacion = @Ubicacion,
                           Fecha = @Fecha, 
                           Organizador = @Organizador,
                           Contacto = @Contacto,
                           ImagenRuta = @ImagenRuta 
                       WHERE Id = @Id";

            await connection.ExecuteAsync(sql, evento);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Organizador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            // Primero eliminar registros relacionados en TEventoFavorito
            await connection.ExecuteAsync(
                "DELETE FROM TEventoFavorito WHERE IdEvento = @Id",
                new { Id = id }
            );

            // Luego eliminar el evento
            await connection.ExecuteAsync(
                "DELETE FROM TEventos WHERE Id = @Id",
                new { Id = id }
            );
        }

        return RedirectToAction(nameof(Index));
    }
}