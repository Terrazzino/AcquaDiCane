namespace AcquaDiCane.Models.DTOs
{
    public class PagoUpdateModel
    {
        public int TurnoId { get; set; }
        public int MetodoDePagoId { get; set; }
        public string CuentaDestino { get; set; }
    }
}
