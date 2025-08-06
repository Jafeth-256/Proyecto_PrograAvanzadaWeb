namespace Proyecto_PrograAvanzadaWeb.Models
{
    public class UsuarioViewModel
    {
        public long IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Identificacion { get; set; }
        public bool Estado { get; set; }
        public int IdRol { get; set; }
        public string NombreRol { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? FotoPath { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string EstadoTexto => Estado ? "Activo" : "Inactivo";
    }

    public class RolViewModel
    {
        public int IdRol { get; set; }
        public string NombreRol { get; set; }
    }
}