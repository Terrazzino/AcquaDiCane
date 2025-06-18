namespace AcquaDiCane.Models
{
    public class Peluquero:Usuario
    {
        public List<Disponibilidad> Disponibilidades { get; set; } = new List<Disponibilidad>();
        public List<Turno> Turnos { get; set; } = new List<Turno>();

        public void AgregarDisponibilidad(Disponibilidad disponibilidad)
        {
            var disp = Disponibilidades.FirstOrDefault(x=>disponibilidad.Dia==x.Dia);
            if (disp==null)
            {
                Disponibilidades.Add(disponibilidad);
            }
        }
    }
}
