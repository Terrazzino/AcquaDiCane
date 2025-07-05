using AcquaDiCane.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

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
        public decimal PrecioTotal { get; set; }
        public string? Observacion { get; set; } // Observaciones generales del turno

        // Nueva propiedad para el estado del turno (string)
        public string Estado { get; set; }

        // Mantengo estas propiedades para manejar la información específica de completado/cancelado.
        // Si Observacion puede abarcar esto, podrías eliminarlas y usar solo Observacion.
        // Pero para diferenciar entre "observaciones generales" y "motivo de cancelación/finalización", es mejor tenerlas.
        public string? MotivoCancelacion { get; set; }
        public string? ObservacionesFinalizacion { get; set; }

        public int? PagoId { get; set; }
        public Pago? Pago { get; set; }

        public void AgregarDetalle(DetalleDelTurno detalle)
        {
            var det = Detalles.FirstOrDefault(x => detalle.ServicioAsignado.Nombre == x.ServicioAsignado.Nombre);
            if (det == null)
            {
                Detalles.Add(detalle);
            }
        }

        public decimal CalcularPrecioTotal()
        {
            return Detalles.Sum(d => d.PrecioServicio);
        }

        public Turno()
        {
            Detalles = new HashSet<DetalleDelTurno>();
            Estado = "Pendiente"; // Valor predeterminado del estado como string
        }
    }
}