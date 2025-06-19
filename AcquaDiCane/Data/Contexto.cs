using AcquaDiCane.Models;
using Microsoft.EntityFrameworkCore;

namespace AcquaDiCane.Data
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<ReciboDePago> ReciboDePagos { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Peluquero> Peluqueros { get; set; }
        public DbSet<JornadaDiaria> Jornadas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Pago) //turno tiene 0 o 1 pago
                .WithOne(p => p.Turno) // un pago pertenece a un turno
                .HasForeignKey<Pago>(p => p.TurnoId); // clave foranea que relaciona el pago con su turno

            modelBuilder.Entity<Turno>()
                .HasMany(t => t.Detalles) //un turno puede incluir varios detalles
                .WithOne(dt => dt.TurnoAsignado)// cada detelle pertenece a un unico turno
                .HasForeignKey(dt => dt.TurnoAsignadoId); // clave foranea que relaciona detalle con turno

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.ReciboDePago) //un pago puede tener 0 o 1 Recibo 
                .WithOne(r => r.Pago)// El recibo depende del pago, solo se genera si el estado del pago es "aprobado"
                .HasForeignKey<ReciboDePago>(r => r.PagoId); // clave foranea que relaciona el recibo con el pago 

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.MetodoDePago) // cada pago se realiza con 1 metodo
                .WithMany(m => m.Pagos) // cada metodo tiene la lista de pagos que se realizaron con ese metodo 
                .HasForeignKey(p => p.MetodoDePagoId);  // clave foranea que relaciona el pago con el metodo utilzado 

            modelBuilder.Entity<Peluquero>()
                .HasMany(p => p.Turnos) //un peluquero puede tener muchos turnos asignados
                .WithOne(t => t.PeluqueroAsignado) // cada turno pertenece a un unico peluquero
                .HasForeignKey(t => t.PeluqueroAsignadoId); // clave foranea que relaciona el turno con el peluquero 

            modelBuilder.Entity<Peluquero>()
                .HasMany(p => p.JornadaSemanal) // un peluquero tiene muchas jornadasDiarias = una jornadaSemanal
                .WithOne(j => j.PeluqueroCorrespondiente) // a cada jornada le corresponde un peluquero
                .HasForeignKey(j => j.PeluqueroCorrespondienteId);// clave foranea que relaciona la jornada con el peluquero 


            modelBuilder.Entity<Servicio>()
                .HasMany(s => s.Detalles)// un servicio puede incluir varios detalles 
                .WithOne(dt => dt.ServicioAsignado) // cada detalle tiene un servicio asignado
                .HasForeignKey(dt => dt.ServicioAsignadoId); // clave foranea que relaciona el servicio de cada detalle con el detalle

            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Mascotas) // un cliente tiene muchas mascotas
                .WithOne(m => m.ClienteAsignado)// una mascota tiene un cliente
                .HasForeignKey(m => m.ClienteAsignadoId); // clave foranea que relaciona la mascota con su respectivo cliente

            modelBuilder.Entity<Mascota>()
                .HasMany(m => m.Turnos) // cada mascota puede tener muchos turnos (historial)
                .WithOne(t => t.MascotaAsignada) // cada turno tiene una unica mascota
                .HasForeignKey(t => t.MascotaAsignadaId); // clave foranea que vincula cada turno con la mascota

        }
    }
}
