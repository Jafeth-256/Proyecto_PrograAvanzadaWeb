using Microsoft.AspNetCore.Mvc;

namespace Proyecto_ProgaAvanzadaWeb_API.Models.DTOs
{
    public class PerfilCompletoDTO
    {
        public long IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Identificacion { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? FotoPath { get; set; }
        public bool Estado { get; set; }
        public int IdRol { get; set; }
        public string NombreRol { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }

    public class ActualizarPerfilBasicoDTO
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Identificacion { get; set; }
    }

    public class ActualizarInformacionAdicionalDTO
    {
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }

    public class CambiarContrasenaPerfilDTO
    {
        public string ContrasenaActual { get; set; }
        public string ContrasenaNueva { get; set; }
    }

    public class SubirFotoPerfilDTO
    {
        public IFormFile Foto { get; set; }
    }

    public class FotoPerfilDTO
    {
        public string FotoPath { get; set; }
    }
}