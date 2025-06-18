using AcquaDiCane.Models.ClasesEnum;

namespace AcquaDiCane.Models
{
    public class Disponibilidad
    {
        public int Id { get; set; }
        public int PeluqueroCorrespondienteId { get; set; }
        public Peluquero PeluqueroCorrespondiente { get; set; }
        public DiasLaborales Dia {  get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFinal {  get; set; }
    }
}
