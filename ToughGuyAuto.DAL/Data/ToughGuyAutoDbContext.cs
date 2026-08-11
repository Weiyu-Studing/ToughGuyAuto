using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToughGuyAuto.Models;

namespace ToughGuyAuto.DAL.Data;

public class ToughGuyAutoDbContext
    : IdentityDbContext<ApplicationUser>
{
    public ToughGuyAutoDbContext(
        DbContextOptions<ToughGuyAutoDbContext> options)
        : base(options) { }

    public DbSet<Vehicle> Vehicles { get; set; }

    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

    public DbSet<ServiceType> ServiceTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Vehicle entity
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

        /* one-to-many relationship:
           one Vehicle can have many MaintenanceRecords, but each MaintenanceRecord belongs to one Vehicle.
           VehicleId is stored in MaintenanceRecord as the foreign key.
           Cascade means deleting a vehicle also deletes all of its maintenance records. */
        builder.Entity<Vehicle>()
            .HasMany(v => v.MaintenanceRecords)
            .WithOne(m => m.Vehicle)
            .HasForeignKey(m => m.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        // MaintenanceRecord entity
        builder.Entity<MaintenanceRecord>(entity =>
        {
            entity.HasKey(m => m.MaintenanceRecordId);

            entity.Property(m => m.ServiceDate)
                .IsRequired();

            entity.Property(m => m.Mileage)
                .IsRequired();

            // Store Cost with up to 10 total digits and 2 digits after the decimal point.
            entity.Property(m => m.Cost)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            entity.Property(m => m.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(m => m.Notes)
                .HasMaxLength(1000);
        });

        /* many-to-many relationship:
           one MaintenanceRecord can contain many ServiceTypes, and one ServiceType can be used by many MaintenanceRecords.
           EF Core creates a join table to store the IDs from both tables. 
         **For example, an oil change and tire rotation can belong to the same maintenance record.
           The join table connects that maintenance record with both service types.*/
        builder.Entity<MaintenanceRecord>()
            .HasMany(m => m.ServiceTypes)
            .WithMany(s => s.MaintenanceRecords)
            .UsingEntity(j =>
                j.ToTable("MaintenanceRecordServiceTypes"));

        // ServiceType entity
        builder.Entity<ServiceType>(entity =>
        {
            entity.HasKey(s => s.ServiceTypeId);

            entity.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(s => s.Description)
                .HasMaxLength(500);

            //The unique index prevents two service types from having the same name.
            entity.HasIndex(s => s.Name)
                .IsUnique();
        });

        // Default service type. We can add, edit, or delete them by logging into the admin account and using the ServiceType function.
        builder.Entity<ServiceType>().HasData(
            new ServiceType
            {
                ServiceTypeId = 1,
                Name = "Oil Change",
                Description = "Change engine oil and oil filter"
            },
            new ServiceType
            {
                ServiceTypeId = 2,
                Name = "Brake Service",
                Description = "Inspect or replace brake components"
            },
            new ServiceType
            {
                ServiceTypeId = 3,
                Name = "Tire Rotation",
                Description = "Rotate vehicle tires"
            },
            new ServiceType
            {
                ServiceTypeId = 4,
                Name = "Battery Replacement",
                Description = "Replace vehicle battery"
            },
            new ServiceType
            {
                ServiceTypeId = 5,
                Name = "Engine Repair",
                Description = "Repair engine related problems"
            }
        );

        /* relationship between Identity users and vehicles 
           one ApplicationUser can own many Vehicles, but each Vehicle belongs to one ApplicationUser. 
           If a user is deleted directly from database, that user's vehicles will also be deleted. */
        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Vehicles)
            .WithOne(v => v.User)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}