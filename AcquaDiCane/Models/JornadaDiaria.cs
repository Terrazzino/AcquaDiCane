
using AcquaDiCane.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcquaDiCane.Models
{
    public class JornadaDiaria
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("PeluqueroCorrespondiente")]
        public int PeluqueroCorrespondienteId { get; set; }
        public Peluquero PeluqueroCorrespondiente { get; set; }
        public string Dia {  get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFinal {  get; set; }

    }
}
