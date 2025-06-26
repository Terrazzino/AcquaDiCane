using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcquaDiCane.Models
{
    public class DetalleDelTurno
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ServicioAsignado")]
        public int ServicioAsignadoId { get; set; }
        public Servicio ServicioAsignado { get; set; }

        [ForeignKey("TurnoAsignado")]
        public int TurnoAsignadoId { get; set; }
        public Turno TurnoAsignado { get; set; }
        public double PrecioServicio { get; set; }
    }
}
