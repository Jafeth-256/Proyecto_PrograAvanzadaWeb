namespace Proyecto_PrograAvanzadaWeb.Models
{
    public class EstadisticasReservasViewModel
    {
        public int TotalReservas { get; set; }
        public int ReservasPendientes { get; set; }
        public int ReservasConfirmadas { get; set; }
        public int ReservasCanceladas { get; set; }
        public decimal IngresosTotales { get; set; }
        public int PersonasTotales { get; set; }

        public string IngresosTotalesFormateado => $"₡{IngresosTotales:N0}";
        public double PorcentajeConfirmadas => TotalReservas > 0 ? (double)ReservasConfirmadas / TotalReservas * 100 : 0;
        public double PorcentajePendientes => TotalReservas > 0 ? (double)ReservasPendientes / TotalReservas * 100 : 0;
        public double PorcentajeCanceladas => TotalReservas > 0 ? (double)ReservasCanceladas / TotalReservas * 100 : 0;
    }
}