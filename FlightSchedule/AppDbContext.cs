using FlightSchedule.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightSchedule
{
    public class AppDbContext : DbContext
    {
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        // Constructor para configurar la cadena de conexión.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aquí puedes realizar configuraciones adicionales de mapeo de modelos a tablas, claves primarias, relaciones, etc.
            // Por ejemplo, si tienes configuraciones de clave primaria personalizadas, puedes definirlas aquí.

            modelBuilder.Entity<Flight>()
                .HasKey(f => f.FlightId); // Reemplaza "Flight" y "FlightId" con los nombres reales de tu entidad y clave primaria.
            modelBuilder.Entity<Route>()
                .HasKey(f => f.RouteId);
            modelBuilder.Entity<Subscription>()
    .HasKey(sub => new { sub.AgencyId, sub.OriginCityId, sub.DestinationCityId });

            modelBuilder.Entity<Flight>()
                   .HasOne(f => f.Route)          // Un vuelo tiene una sola ruta
                   .WithMany()                   // Indicamos que Route no tiene una propiedad de navegación
                   .HasForeignKey(f => f.RouteId); // Clave foránea en Flight (RouteId)

            modelBuilder.Entity<Route>()
                .HasMany(r => r.Flights)     // Una ruta puede tener muchos vuelos
                .WithOne(f => f.Route)      // Un vuelo tiene una sola ruta
                .HasForeignKey(f => f.RouteId); // Clave foránea en Flight (RouteId)
        }
    }
}
