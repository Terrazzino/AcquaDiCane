namespace AcquaDiCane.Models
{
    public class ReciboDePago
    {
        public int Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal Monto { get; set; }
        public int PagoId { get; set; }
        public Pago Pago { get; set; }
    }
}
