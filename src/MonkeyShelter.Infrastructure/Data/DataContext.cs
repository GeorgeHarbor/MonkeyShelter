using Microsoft.EntityFrameworkCore;

using MonkeyShelter.Domain;

namespace MonkeyShelter.Infrastructure;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Species> Species { get; set; }
    public DbSet<Shelter> Shelters { get; set; }
    public DbSet<Monkey> Monkeys { get; set; }

    public DbSet<Arrival> Arrivals { get; set; }
    public DbSet<Departure> Departures { get; set; }
    public DbSet<WeightHistory> WeightHistories { get; set; }

    public DbSet<VetCheckSchedule> VetCheckSchedules { get; set; }
    public DbSet<ReportCache> ReportCaches { get; set; }

    public DbSet<ManagerShelter> ManagerShelters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ManagerShelter>(b =>
           {
               b.ToTable("ManagerShelters");

               b.HasKey(ms => new { ms.ManagerId, ms.ShelterId });

               b.HasOne(ms => ms.Shelter)
                .WithMany(s => s.ManagerShelters)
                .HasForeignKey(ms => ms.ShelterId)
                .OnDelete(DeleteBehavior.Cascade);
           });
    }
}