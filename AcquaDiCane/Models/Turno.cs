namespace AcquaDiCane.Models
{
    public class Turno
    {
        public int Id { get; set; }
        public int MascotaAsignadaId { get; set; }
        public Mascota MascotaAsignada { get; set; }
        public int PeluqueroAsignadoId { get; set; }
        public Peluquero PeluqueroAsignado { get; set; }
        public DateTime FechaHoraDelTurno { get; set; }
        public List<DetalleDelTurno> Detalles { get; set; } = new List<DetalleDelTurno>();
        public double PrecioTotal { get; set; }
        public string Observacion { get; set; }
        public Pago Pago { get; set; }

        public void AgregarDetalle(DetalleDelTurno detalle)
        {
            var det = Detalles.FirstOrDefault(x=> detalle.ServicioAsignado.TipoDeServicio == x.ServicioAsignado.TipoDeServicio);
            if (det==null)
            {
                Detalles.Add(detalle);
            }
        }

        public double CalcularPrecioTotal()
        {
            return Detalles.Sum(d=>d.ServicioAsignado.Precio);
        }
    }
}
