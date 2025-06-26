using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcquaDiCane.Models
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }
        public double Monto { get; set; }
        public DateTime? FechaPago { get; set; }
        public string Estado { get; set; } // "Aprobado", "Pendiente", "Rechazado", "Cancelado"
        public string MercadoPagoPreferenceId { get; set; } // ID que devuelve MP
        
        [ForeignKey("Turno")]
        public int TurnoId { get; set; }
        public Turno Turno { get; set; }

        [ForeignKey("MetodoDePago")]
        public int MetodoDePagoId { get; set; }
        public MetodoDePago MetodoDePago { get; set; }
        public string CuentaDestino { get; set; }
    }

}
