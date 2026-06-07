using DivaSalon.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DivaSalon.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Barber> Barbers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configurare pentru decimal
            builder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            builder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasPrecision(18, 2);

            builder.Entity<Service>()
                .Property(s => s.PriceFrom)
                .HasPrecision(18, 2);

            builder.Entity<Service>()
                .Property(s => s.PriceTo)
                .HasPrecision(18, 2);

            builder.Entity<Order>()
                .HasOne(o => o.Barber)
                .WithMany()
                .HasForeignKey(o => o.BarberId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Service)
                .WithMany()
                .HasForeignKey(oi => oi.ServiceId);
        }
    }
}