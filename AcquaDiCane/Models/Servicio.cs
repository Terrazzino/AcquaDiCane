namespace AcquaDiCane.Models
{
    public class Servicio
    {
        public int Id { get; set; }
        public string nombreServicio { get; set; }
        public double Precio { get; set; }
        public double Duracion { get; set; }
        public List<DetalleDelTurno> Detalles { get; set; } = new List<DetalleDelTurno>();
    }
}
