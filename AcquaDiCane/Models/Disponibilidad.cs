
namespace AcquaDiCane.Models
{
    public class JornadaDiaria
    {
        public int Id { get; set; }
        public int PeluqueroCorrespondienteId { get; set; }
        public Peluquero PeluqueroCorrespondiente { get; set; }
        public string Dia {  get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFinal {  get; set; }
    }
}
