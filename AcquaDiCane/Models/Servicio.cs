// Models/Servicio.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Para [Column] si usas decimal

namespace AcquaDiCane.Models
{
    public class Servicio
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; } // Renombrado de nombreServicio a Nombre para consistencia con DTO
        public string Descripcion { get; set; } // Nueva propiedad, si la quieres persistir
        public decimal Precio { get; set; } // Cambiado de double a decimal
        public int DuracionEnMinutos { get; set; } // Cambiado de double Duracion a int DuracionEnMinutos


        // Propiedad de navegación que ya tienes
        public ICollection<DetalleDelTurno> Detalles { get; set; }

        public Servicio()
        {
            Detalles = new HashSet<DetalleDelTurno>();
        }
    }
}