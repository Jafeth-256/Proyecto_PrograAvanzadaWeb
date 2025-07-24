namespace Proyecto_PrograAvanzadaWeb.Models
{
    public class PerfilUsuario
    {
        public long IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Identificacion { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string FotoPath { get; set; }
        public bool Estado { get; set; }
        public int IdRol { get; set; }
        public string NombreRol { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }

    public class ActualizarPerfilBasico
    {
        public long IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Identificacion { get; set; }
    }

    public class InformacionAdicional
    {
        public long IdUsuario { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string FotoPath { get; set; }
    }

    public class CambiarContrasena
    {
        public long IdUsuario { get; set; }
        public string ContrasenaActual { get; set; }
        public string ContrasenaNueva { get; set; }
        public string ConfirmarContrasena { get; set; }
    }
}