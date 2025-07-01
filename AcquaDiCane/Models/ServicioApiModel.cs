// Modelo para la creación/actualización de un servicio
public class ServicioApiModel
{
    public int Id { get; set; } // 0 para creación, ID para actualización
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int DuracionEnMinutos { get; set; } // Duración estimada del servicio
}