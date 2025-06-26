using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcquaDiCane.Models
{
    public class ReciboDePago
    {
        [Key]
        public int Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public double Monto { get; set; }

        [ForeignKey("Pago")]
        public int PagoId { get; set; }
        public Pago Pago { get; set; }
    }
}
