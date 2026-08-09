using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToughGuyAuto.Models;

namespace ToughGuyAuto.DAL.Data;

public class ToughGuyAutoDbContext
    : IdentityDbContext<ApplicationUser>
{
    public ToughGuyAutoDbContext(
        DbContextOptions<ToughGuyAutoDbContext> options)
        : base(options)
    {

    }

    public DbSet<Vehicle> Vehicles { get; set; }

    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

    public DbSet<ServiceType> ServiceTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Vehicle
        builder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(v => v.VehicleId);

                entity.Property(v => v.Make)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(v => v.Model)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(v => v.Year)
                    .IsRequired();

                entity.Property(v => v.LicensePlate)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(v => v.VIN)
                    .IsRequired()
                    .HasMaxLength(17);

                entity.Property(v => v.Mileage)
                    .IsRequired();
            });

        // Vehicle to MaintenanceRecord
        builder.Entity<Vehicle>()
        .HasMany(v => v.MaintenanceRecords)
        .WithOne(m => m.Vehicle)
        .HasForeignKey(m => m.VehicleId)
        .OnDelete(DeleteBehavior.Cascade);

        // MaintenanceRecord
        builder.Entity<MaintenanceRecord>(entity =>
        {
            entity.HasKey(m => m.MaintenanceRecordId);

            entity.Property(m => m.ServiceDate)
                .IsRequired();

            entity.Property(m => m.Mileage)
                .IsRequired();

            entity.Property(m => m.Cost)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            entity.Property(m => m.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(m => m.Notes)
                .HasMaxLength(1000);
        });

        // MaintenanceRecord to ServiceType
        builder.Entity<MaintenanceRecord>()
        .HasMany(m => m.ServiceTypes)
        .WithMany(s => s.MaintenanceRecords)
        .UsingEntity(j =>
        j.ToTable("MaintenanceRecordServiceTypes"));

        // ServiceType
        builder.Entity<ServiceType>(entity =>
        {
            entity.HasKey(s => s.ServiceTypeId);

            entity.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(s => s.Description)
                .HasMaxLength(500);

            entity.HasIndex(s => s.Name)
                .IsUnique();
        });

        // ApplicationUser to Vehicle
        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Vehicles)
            .WithOne()
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Vehicles)
            .WithOne(v => v.User)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
