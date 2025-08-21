using System.ComponentModel.DataAnnotations;

namespace Proyecto_ProgaAvanzadaWeb_API.Models.DTOs
{
    public class TourDTO
    {
        public long IdTour { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Destino { get; set; }
        public decimal Precio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int CantidadPersonas { get; set; }
        public string? NombreCreador { get; set; }
    }

    public class CrearTourDTO
    {
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public string Destino { get; set; }

        public decimal Precio { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public int CantidadPersonas { get; set; }
    }
}