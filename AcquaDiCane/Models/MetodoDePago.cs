using System.ComponentModel.DataAnnotations;

namespace AcquaDiCane.Models
{
    public class MetodoDePago
    {
        public int Id { get; set; }
        public string NombreDelMetodo { get; set; }

        public List<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
