using Kolokwium_CodeFirst.Models;

namespace Kolokwium_CodeFirst.Context;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Clients");
            entity.HasKey(e => e.IdClient);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.ToTable("Subscriptions");
            entity.HasKey(e => e.IdSubscription);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("Sales");
            entity.HasKey(e => e.IdSale);
            entity.HasOne(e => e.Client)
                .WithMany(c => c.Sales)
                .HasForeignKey(e => e.IdClient);
            entity.HasOne(e => e.Subscription)
                .WithMany(s => s.Sales)
                .HasForeignKey(e => e.IdSubscription);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payments");
            entity.HasKey(e => e.IdPayment);
            entity.HasOne(e => e.Sale)
                .WithMany(s => s.Payments)
                .HasForeignKey(e => e.IdSale);
        });
    }
}