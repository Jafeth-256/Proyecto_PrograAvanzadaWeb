using Microsoft.AspNetCore.Mvc;

namespace Proyecto_PrograAvanzadaWeb.Models
{
    public class CrearEventoUniversitarioViewModel
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public string Universidad { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public DateTime FechaInicio { get; set; } = DateTime.Now.AddDays(1);
        public DateTime FechaFin { get; set; } = DateTime.Now.AddDays(1).AddHours(3);
        public int CantidadPersonas { get; set; } = 100;
    }
}
