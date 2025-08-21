namespace Proyecto_PrograAvanzadaWeb.Models
{
    public class PerfilUsuarioCompleto
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

    public class ActualizarPerfilBasicoModel
    {
        public long IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Identificacion { get; set; }
    }

    public class InformacionAdicionalModel
    {
        public long IdUsuario { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? FotoPath { get; set; }
        public IFormFile? Foto { get; set; }
    }

    public class CambiarContrasenaModel
    {
        public long IdUsuario { get; set; }
        public string ContrasenaActual { get; set; }
        public string ContrasenaNueva { get; set; }
        public string ConfirmarContrasena { get; set; }
    }
}