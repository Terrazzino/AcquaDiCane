using AcquaDiCane.Models;
using AcquaDiCane.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AcquaDiCane.Data
{
    public class Contexto : IdentityDbContext<AplicationUser, IdentityRole<int>, int>
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Peluquero> Peluqueros { get; set; }
        public DbSet<DetalleDelTurno> DetallesDeTurnos { get; set; }
        public DbSet<JornadaDiaria> JornadasSemanales { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<MetodoDePago> MetodosDePago { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<ReciboDePago> RecibosDePago { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Turno> Turnos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AplicationUser>()
                .HasOne(u => u.PerfilCliente)
                .WithOne(c => c.AplicationUser)
                .HasForeignKey<Cliente>(c => c.AplicationUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AplicationUser>()
                .HasOne(u => u.PerfilPeluquero)
                .WithOne(p => p.AplicationUser)
                .HasForeignKey<Peluquero>(p => p.AplicationUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Mascota>()
                .HasOne(m => m.ClienteAsignado)
                .WithMany(c => c.Mascotas)
                .HasForeignKey(m => m.ClienteAsignadoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JornadaDiaria>()
                .HasOne(j => j.PeluqueroCorrespondiente)
                .WithMany(p => p.JornadaSemanal)
                .HasForeignKey(j => j.PeluqueroCorrespondienteId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Turno>()
                .HasOne(t => t.PeluqueroAsignado)
                .WithMany(p => p.Turnos)
                .HasForeignKey(t => t.PeluqueroAsignadoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DetalleDelTurno>()
                .HasOne(d => d.TurnoAsignado)
                .WithMany(t => t.Detalles)
                .HasForeignKey(d => d.TurnoAsignadoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DetalleDelTurno>()
                .HasOne(d => d.ServicioAsignado)
                .WithMany(t => t.Detalles)
                .HasForeignKey(d => d.ServicioAsignadoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Turno>()
                .HasOne(t => t.MascotaAsignada)
                .WithMany(m => m.Turnos) // ← cambio acá
                .HasForeignKey(t => t.MascotaAsignadaId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Turno>()
               .HasOne(t => t.Pago)
               .WithOne()
               .HasForeignKey<Turno>(t => t.PagoId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<ReciboDePago>()
                .HasOne(r => r.Pago)
                .WithOne()
                .HasForeignKey<ReciboDePago>(r => r.PagoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Pago>()
                .HasOne(p => p.MetodoDePago)
                .WithMany(m => m.Pagos)
                .HasForeignKey(p => p.MetodoDePagoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MetodoDePago>().HasData(
                new MetodoDePago { Id = 1, NombreDelMetodo = "Efectivo" },
                new MetodoDePago { Id = 2, NombreDelMetodo = "MercadoPago" },
                new MetodoDePago { Id = 3, NombreDelMetodo = "Pendiente" }
            );

            // Configuración de decimales para precios, para asegurar precisión en campos de dinero
            builder.Entity<Pago>()
                .Property(p => p.Monto)
                .HasColumnType("decimal(18,2)");

        }
    }
}
