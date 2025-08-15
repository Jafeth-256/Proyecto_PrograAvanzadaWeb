namespace Proyecto_ProgaAvanzadaWeb_API.Models.Entities
{
    public class Usuario
    {
        public long IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Identificacion { get; set; }
        public string Contrasenna { get; set; }
        public bool Estado { get; set; }
        public int IdRol { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? FotoPath { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string? NombreRol { get; set; }
    }
}
