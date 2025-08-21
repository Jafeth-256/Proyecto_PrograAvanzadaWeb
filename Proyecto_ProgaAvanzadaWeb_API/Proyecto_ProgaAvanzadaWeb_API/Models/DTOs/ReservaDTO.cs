namespace Proyecto_ProgaAvanzadaWeb_API.Models.DTOs
{
    public class CrearReservaDTO
    {
        public long IdTour { get; set; }
        public int CantidadPersonas { get; set; }
        public string? Comentarios { get; set; }
    }

    public class EstadisticasReservasDTO
    {
        public int TotalReservas { get; set; }
        public int ReservasPendientes { get; set; }
        public int ReservasConfirmadas { get; set; }
        public int ReservasCanceladas { get; set; }
        public decimal IngresosTotales { get; set; }
        public int PersonasTotales { get; set; }
    }
}