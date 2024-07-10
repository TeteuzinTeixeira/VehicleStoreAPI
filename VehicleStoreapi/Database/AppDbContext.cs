using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VehicleStoreapi.Database.Vehicle;
using VehicleStoreapi.Model.Entities;

namespace VehicleStoreapi.Database
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
            
        public DbSet<Vehicle.Vehicle> Vehicle { get; set; }
        public DbSet<VehicleImage> VehicleImage { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuração de UUID
            builder.Entity<Vehicle.Vehicle>()
                .Property(v => v.Id)
                .HasDefaultValueSql("public.uuid_generate_v4()");
    
            builder.Entity<VehicleImage>()
                .Property(v => v.Id)
                .HasDefaultValueSql("public.uuid_generate_v4()");
    
            builder.Entity<Order>()
                .Property(v => v.Id)
                .HasDefaultValueSql("public.uuid_generate_v4()");
    
            builder.Entity<OrderVehicleLink>()
                .Property(v => v.Id)
                .HasDefaultValueSql("public.uuid_generate_v4()");
    
            // Configuração de relacionamento
            builder.Entity<Vehicle.Vehicle>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .IsRequired();
    
            builder.Entity<OrderVehicleLink>()
                .HasOne<Vehicle.Vehicle>()
                .WithMany()
                .HasForeignKey(v => v.VehicleId)
                .IsRequired();
    
            builder.Entity<OrderVehicleLink>()
                .HasOne<Order>()
                .WithMany()
                .HasForeignKey(v => v.OrderId)
                .IsRequired();

            builder.Entity<Vehicle.Vehicle>()
                .HasMany(v => v.Images)
                .WithOne(i => i.Vehicle)
                .HasForeignKey(i => i.VehicleId);
    
            // Configuração da propriedade Data como bytea
            builder.Entity<VehicleImage>()
                .Property(vi => vi.Data)
                .HasColumnType("bytea");

            // Configuração de schema
            builder.HasDefaultSchema("Store");
        }
    }
}