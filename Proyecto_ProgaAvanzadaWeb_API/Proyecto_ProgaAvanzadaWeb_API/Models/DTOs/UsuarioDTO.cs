using System.ComponentModel.DataAnnotations;

namespace Proyecto_ProgaAvanzadaWeb_API.Models.DTOs
{
    public class UsuarioDTO
    {
        public long IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Identificacion { get; set; }
        public bool Estado { get; set; }
        public int IdRol { get; set; }
        public string? NombreRol { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? FotoPath { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }

    public class RegistroUsuarioDTO
    {
        public string Nombre { get; set; }

        public string Correo { get; set; }

        public string Identificacion { get; set; }

        public string Contrasenna { get; set; }
    }

    public class ActualizarPerfilDTO
    {
        public string Nombre { get; set; }

        public string Correo { get; set; }

        public string Identificacion { get; set; }
    }

    public class CambiarContrasenaDTO
    {
        public string ContrasenaActual { get; set; }

        public string ContrasenaNueva { get; set; }
    }

    public class CambiarEstadoUsuarioDTO
    {
        public bool Estado { get; set; }
    }

    public class ActualizarUsuarioCompletoDTO
    {
        public string Nombre { get; set; }

        public string Correo { get; set; }

        public string Identificacion { get; set; }

        public bool Estado { get; set; }

        public int IdRol { get; set; }
    }

    public class EstadisticasUsuariosDTO
    {
        public int TotalUsuarios { get; set; }
        public int UsuariosActivos { get; set; }
        public int UsuariosInactivos { get; set; }
        public int UsuariosRegulares { get; set; }
        public int UsuariosAdministradores { get; set; }
    }
}