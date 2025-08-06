using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_ProgaAvanzadaWeb_API.Models.DTOs;
using Proyecto_ProgaAvanzadaWeb_API.Services;
using System.Security.Claims;

namespace Proyecto_ProgaAvanzadaWeb_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var result = await _usuarioService.ObtenerTodos();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            var result = await _usuarioService.ObtenerPorId(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("perfil")]
        public async Task<IActionResult> ObtenerPerfil()
        {
            var idUsuario = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _usuarioService.ObtenerPorId(idUsuario);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("perfil")]
        public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarPerfilDTO dto)
        {
            var idUsuario = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _usuarioService.ActualizarPerfil(idUsuario, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("cambiar-contrasena")]
        public async Task<IActionResult> CambiarContrasena([FromBody] CambiarContrasenaDTO dto)
        {
            var idUsuario = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _usuarioService.CambiarContrasena(idUsuario, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/estado")]
        [Authorize(Roles = "Usuario Administrador")]
        public async Task<IActionResult> CambiarEstado(long id, [FromBody] bool nuevoEstado)
        {
            var result = await _usuarioService.CambiarEstado(id, nuevoEstado);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}