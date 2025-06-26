using System.ComponentModel.DataAnnotations;

namespace AcquaDiCane.Models
{
    public class MetodoDePago
    {
        [Key]
        public int Id { get; set; }
        public string NombreDelMetodo { get; set; }

        public ICollection<Pago> Pagos { get; set; }
        public MetodoDePago()
        {
            Pagos = new HashSet<Pago>();
        }
    }
}
