namespace AcquaDiCane.Models
{
    public class Peluquero:Usuario
    {
        public List<Disponibilidad> Disponibilidades { get; set; } = new List<Disponibilidad>();

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
