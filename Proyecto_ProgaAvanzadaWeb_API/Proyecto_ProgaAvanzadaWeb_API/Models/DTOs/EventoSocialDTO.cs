using System.ComponentModel.DataAnnotations;

namespace Proyecto_ProgaAvanzadaWeb_API.Models.DTOs
{
    public class EventoSocialDTO
    {
        public long IdEventoSocial { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Ubicacion { get; set; }
        public decimal Precio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int CantidadPersonas { get; set; }
        public string? NombreCreador { get; set; }
    }

    public class CrearEventoSocialDTO
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public string Ubicacion { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        public int CantidadPersonas { get; set; }
    }
}