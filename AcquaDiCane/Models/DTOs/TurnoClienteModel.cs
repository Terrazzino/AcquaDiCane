public class TurnoClienteModel
{
    public int Id { get; set; }

    public string PetName { get; set; }

    public string GroomerName { get; set; }

    public DateTime Date { get; set; }  // Fecha del turno (sin hora)
    public string Time { get; set; }    // Hora en formato string, ej. "14:30"

    public string ServiceName { get; set; }

    public string Status { get; set; } // Estado del turno: "Pendiente", "Finalizado", etc.

    public string? MetodoDePago { get; set; } // Efectivo, MercadoPago, etc.
    public string? PagoEstado { get; set; }   // Estado del pago: Pendiente, Aprobado, etc.

    public bool MostrarBotonPago { get; set; } // Si debe mostrarse el botón "Pagar"
}
