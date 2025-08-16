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
        [Required(ErrorMessage = "El nombre del tour es obligatorio")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder 255 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El destino es obligatorio")]
        [StringLength(255, ErrorMessage = "El destino no puede exceder 255 caracteres")]
        public string Destino { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "La cantidad de personas es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de personas debe ser mayor a 0")]
        public int CantidadPersonas { get; set; }
    }
}