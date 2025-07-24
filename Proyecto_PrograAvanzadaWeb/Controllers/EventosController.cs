using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class EventosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<IdentityUser> _userManager;

    public EventosController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _env = env;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var eventos = await _context.Eventos.OrderBy(e => e.Fecha).ToListAsync();
        return View(eventos);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var evento = await _context.Eventos.FindAsync(id);
        if (evento == null) return NotFound();
        return View(evento);
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
            _context.Add(evento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(evento);
    }

    [Authorize(Roles = "Organizador")]
    public async Task<IActionResult> Editar(int id)
    {
        var evento = await _context.Eventos.FindAsync(id);
        if (evento == null) return NotFound();
        return View(evento);
    }

    [HttpPost]
    [Authorize(Roles = "Organizador")]
    public async Task<IActionResult> Editar(Evento evento)
    {
        _context.Update(evento);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Organizador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var evento = await _context.Eventos.FindAsync(id);
        if (evento == null) return NotFound();
        _context.Eventos.Remove(evento);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
