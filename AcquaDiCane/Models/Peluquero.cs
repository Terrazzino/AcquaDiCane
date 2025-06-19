namespace AcquaDiCane.Models
{
    public class Peluquero:Usuario
    {
        public List<JornadaDiaria> JornadaSemanal { get; set; } = new List<JornadaDiaria>();
        public List<Turno> Turnos { get; set; } = new List<Turno>();

        public void AgregarDisponibilidad(JornadaDiaria jornada)
        {
            var jor = JornadaSemanal.FirstOrDefault(x=>jornada.Dia==x.Dia);
            if (jor==null)
            {
                JornadaSemanal.Add(jornada);
            }
        }
    }
}
