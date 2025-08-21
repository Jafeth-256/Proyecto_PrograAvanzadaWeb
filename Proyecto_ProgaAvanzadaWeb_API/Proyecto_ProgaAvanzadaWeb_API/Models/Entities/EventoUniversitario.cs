namespace Proyecto_ProgaAvanzadaWeb_API.Models.Entities
{
    public class EventoUniversitario
    {
        public long IdEventoUniversitario { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Ubicacion { get; set; }
        public string Universidad { get; set; }
        public decimal Precio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int CantidadPersonas { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public long IdUsuarioCreador { get; set; }
        public string? NombreCreador { get; set; }
    }
}