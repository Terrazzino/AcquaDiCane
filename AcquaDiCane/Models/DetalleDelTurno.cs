namespace AcquaDiCane.Models
{
    public class DetalleDelTurno
    {
        public int Id { get; set; }
        public int ServicioAsignadoId { get; set; }
        public Servicio ServicioAsignado { get; set; }
        public int TurnoAsignadoId { get; set; }
        public Turno TurnoAsignado { get; set; }
    }
}
