namespace AcquaDiCane.Models
{
    public class Cliente:Usuario
    {
        public List<Mascota> Mascotas { get; set; } = new List<Mascota>();

        public void AgregarMascota(Mascota mascota)
        {
            var mas = Mascotas.FirstOrDefault(x=>mascota.Id==x.Id);
            if (mas==null)
            {
                Mascotas.Add(mascota);
            }
        }
    }
}
