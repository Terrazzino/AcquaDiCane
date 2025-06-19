namespace AcquaDiCane.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string Estado { get; set; } // "Aprobado", "Pendiente", "Rechazado", "Cancelado"
        public string MercadoPagoPreferenceId { get; set; } // ID que devuelve MP
        public int TurnoId { get; set; }
        public Turno Turno { get; set; }
        public ReciboDePago ReciboDePago { get; set; }
        public MetodoDePago MetodoDePago { get; set; }
        public int MetodoDePagoId { get; set; }
        public string CuentaDestino { get; set; }
    }

}
