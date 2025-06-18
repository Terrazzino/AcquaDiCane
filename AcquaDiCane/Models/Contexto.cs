using Microsoft.EntityFrameworkCore;

namespace AcquaDiCane.Models
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
        public DbSet<Disponibilidad> Disponibilidades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Pago) //turno tiene 0 o 1 pago
                .WithOne(p => p.Turno) // un pago pertenece a un turno
                .HasForeignKey<Pago>(p => p.TurnoId); // clave foranea que relaciona el pago con su turno

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.ReciboDePago) //un pago puede tener 0 o 1 Recibo 
                .WithOne(r => r.Pago)// El recibo depende del pago, solo se genera si el estado del pago es "aprobado"
                .HasForeignKey<ReciboDePago>(r => r.PagoId); // clave foranea que relaciona el recibo con el pago 

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.MetodoDePago) // cada pago se realiza con 1 metodo
                .WithMany() 
                .HasForeignKey(p => p.MetodoDePagoId);  // clave foranea que relaciona el pago con el metodo utilzado 

            modelBuilder.Entity<Peluquero>()
                .HasMany(p => p.Disponibilidades) // un peluquero puede tener muchas disponibilidades
                .WithOne()
                .HasForeignKey();

            modelBuilder.Entity<Peluquero>()
                .HasMany(p => p.Turnos) //un peluquero puede tener muchos turnos asignados
                .WithOne(t => t.PeluqueroAsignado) // cada turno pertenece a un unico peluquero
                .HasForeignKey(t => t.PeluqueroAsignadoId); // clave foranea que relaciona el turno con el peluquero 

            modelBuilder.Entity<Turno>()
                .HasMany(t => t.Detalles) //un turno puede incluir varios servicios
                .WithOne(dt => dt.TurnoAsignado)// cada detelle pertenece a un unico turno
                .HasForeignKey(dt => dt.TurnoAsignadoId); // clave foranea que relaciona detalle con turno

                // falta Servicio - Detalles turno
                // falta Peluquero - Disponibilidades 


        }
    }
}
