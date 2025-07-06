using System.ComponentModel.DataAnnotations;

public class PagoUpdateDto
{
    [Required(ErrorMessage = "El método de pago es obligatorio.")]
    public int MetodoDePagoId { get; set; }

    public string? CuentaDestino { get; set; } // opcional, se usa si es MercadoPago

    public DateTime FechaPago { get; set; } = DateTime.Now;

    public string Estado { get; set; } = "Aprobado"; // se podría dejar fijo o validarlo desde el servidor
}
