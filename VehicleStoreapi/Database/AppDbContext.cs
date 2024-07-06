using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VehicleStoreapi.Database.Vehicle;

namespace VehicleStoreapi.Database
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Vehicle.Vehicle>()
                .Property(v => v.Id)
                .HasDefaultValueSql("public.uuid_generate_v4()");
            
            builder.Entity<VehicleImage>()
                .Property(v => v.Id)
                .HasDefaultValueSql("public.uuid_generate_v4()");
            
            builder.Entity<Order>()
                .Property(v => v.Id)
                .HasDefaultValueSql("public.uuid_generate_v4()");
            
            builder.Entity<Vehicle.Vehicle>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .IsRequired();
            
            builder.Entity<Vehicle.Vehicle>()
                .HasOne(v => v.Order)
                .WithMany()
                .HasForeignKey(v => v.OrderId) // Assuming OrderId in Vehicle is Guid
                .IsRequired();

            
            builder.Entity<VehicleImage>()
                .HasOne(v => v.Vehicle)
                .WithMany()
                .HasForeignKey(v => v.VehicleId)
                .IsRequired();
            
            builder.HasDefaultSchema("Store");
        }
    }
}