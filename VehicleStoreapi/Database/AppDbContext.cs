﻿using Microsoft.AspNetCore.Identity;
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
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderVehicleLink> OrderVehicleLink { get; set; }

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
            builder.Entity<OrderVehicleLink>()
                .HasOne<Vehicle.Vehicle>()
                .WithMany()
                .HasForeignKey(v => v.VehicleId)
                .IsRequired();

            builder.Entity<OrderVehicleLink>()
                .HasKey(ovl => ovl.Id);

            builder.Entity<VehicleImage>()
                .HasOne<Vehicle.Vehicle>()
                .WithMany()
                .HasForeignKey(v => v.VehicleId)
                .IsRequired();

            // Configuração de schema
            builder.HasDefaultSchema("Store");
        }
    }
}
