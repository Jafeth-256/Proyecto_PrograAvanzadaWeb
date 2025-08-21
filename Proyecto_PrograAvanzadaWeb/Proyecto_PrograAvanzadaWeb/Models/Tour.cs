using System.ComponentModel.DataAnnotations;

namespace Proyecto_PrograAvanzadaWeb.Models
{
    /// <summary>
    /// ViewModel para mostrar información de tours
    /// </summary>
    public class TourViewModel
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

        // Propiedades computadas para la vista
        public string FechaInicioFormateada => FechaInicio.ToString("dd/MM/yyyy");
        public string FechaFinFormateada => FechaFin.ToString("dd/MM/yyyy");
        public string PrecioFormateado => Precio.ToString("C", new System.Globalization.CultureInfo("es-CR"));
        public int DuracionEnDias => (FechaFin - FechaInicio).Days;
        public bool EsTourFuturo => FechaInicio > DateTime.Now;
        public bool EsTourEnCurso => DateTime.Now >= FechaInicio && DateTime.Now <= FechaFin;
        public bool TourTerminado => FechaFin < DateTime.Now;

        public string EstadoTour
        {
            get
            {
                if (TourTerminado) return "Finalizado";
                if (EsTourEnCurso) return "En curso";
                return "Próximamente";
            }
        }
    }

    public class CrearTourViewModel
    {
        public string Nombre { get; set; }


        public string Descripcion { get; set; }


        public string Destino { get; set; }


        public decimal Precio { get; set; }


        public DateTime FechaInicio { get; set; } = DateTime.Now.AddDays(1);


        public DateTime FechaFin { get; set; } = DateTime.Now.AddDays(8);

        public int CantidadPersonas { get; set; }
    }

    public class EditarTourViewModel
    {
        public long IdTour { get; set; }


        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public string Destino { get; set; }


        public decimal Precio { get; set; }


        public DateTime FechaInicio { get; set; }


        public DateTime FechaFin { get; set; }

        public int CantidadPersonas { get; set; }
    }
}