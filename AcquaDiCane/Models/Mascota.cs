
using AcquaDiCane.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcquaDiCane.Models
{
    public class Mascota
    {
        [Key]
        public int Id { get; set; }
        public string UrlFotoPerfil { get; set; }
        public string Nombre { get; set; }

        [ForeignKey("ClienteAsignado")]
        public int ClienteAsignadoId { get; set; }
        public Cliente ClienteAsignado { get; set; }
        public string Tamaño { get; set; }
        public string Sexo {  get; set; }
        public bool SinRaza { get; set; }
        public bool Castrado { get; set; }
        public bool Alergico { get; set; }
        public string Raza { get; set; }
        public double Peso { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public Turno Turno { get; set; }
    }
}
