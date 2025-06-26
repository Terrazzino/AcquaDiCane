using AcquaDiCane.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcquaDiCane.Models
{
    public class Turno
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("MascotaAsignada")]
        public int MascotaAsignadaId { get; set; }
        public Mascota MascotaAsignada { get; set; }

        [ForeignKey("PeluqueroAsignado")]
        public int PeluqueroAsignadoId { get; set; }
        public Peluquero PeluqueroAsignado { get; set; }
        public DateTime FechaHoraDelTurno { get; set; }
        public ICollection<DetalleDelTurno> Detalles { get; set; }
        public double PrecioTotal { get; set; }
        public string Observacion { get; set; }
        public Pago Pago { get; set; }

        public void AgregarDetalle(DetalleDelTurno detalle)
        {
            var det = Detalles.FirstOrDefault(x=> detalle.ServicioAsignado.nombreServicio == x.ServicioAsignado.nombreServicio);
            if (det==null)
            {
                Detalles.Add(detalle);
            }
        }

        public double CalcularPrecioTotal()
        {
            return Detalles.Sum(d=>d.PrecioServicio);
        }

        public Turno()
        {
            Detalles = new HashSet<DetalleDelTurno>();
        }
    }
}
