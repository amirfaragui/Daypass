using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace ValueCards.Data
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Operator> Operators { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    public DbSet<AmountThreshold> AmountThresholds { get; set; }
    public DbSet<QuantityThreshold> QuantityThresholds { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<Tenant>()
        .HasMany(e => e.Reservations)
        .WithOne(e => e.Tenant)
        .HasForeignKey(e => e.TenantId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.Entity<Tenant>()
        .HasMany(e => e.AmountThresholds)
        .WithOne(e => e.Tenant)
        .HasForeignKey(e => e.TenantId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.Entity<Tenant>()
        .HasMany(e => e.QuantityThresholds)
        .WithOne(e => e.Tenant)
        .HasForeignKey(e => e.TenantId)
        .OnDelete(DeleteBehavior.Cascade);


      builder.Entity<Tenant>()
        .HasIndex(e => e.QRCodeSeed)
        .IsUnique();

      //builder.ApplyConfiguration(new ApplicationUserConfiguration());
    }
  }
}
