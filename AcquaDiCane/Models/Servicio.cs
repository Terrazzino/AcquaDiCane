using System.ComponentModel.DataAnnotations;

namespace AcquaDiCane.Models
{
    public class Servicio
    {
        [Key]
        public int Id { get; set; }
        public string nombreServicio { get; set; }
        public double Precio { get; set; }
        public double Duracion { get; set; }
        public ICollection<DetalleDelTurno> Detalles { get; set; }

        public Servicio()
        {
            Detalles = new HashSet<DetalleDelTurno>();
        }
    }
}
