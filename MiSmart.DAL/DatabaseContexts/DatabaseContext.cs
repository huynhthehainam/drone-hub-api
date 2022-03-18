using MiSmart.DAL.Models;
using Microsoft.EntityFrameworkCore;
namespace MiSmart.DAL.DatabaseContexts
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TelemetryRecord> TelemetryRecords { get; set; }
        public DbSet<TeamUser> TeamUsers { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<FlightStat> FlightStats { get; set; }
        public DbSet<DeviceModel> DeviceModels { get; set; }
        public DbSet<CustomerUser> CustomerUsers { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<TelemetryGroup> TelemetryGroups { get; set; }
        public DbSet<LogFile> LogFiles { get; set; }
        public DbSet<ExecutionCompany> ExecutionCompanies { get; set; }
        public DbSet<ExecutionCompanyUser> ExecutionCompanyUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("postgis");
            modelBuilder.Entity<Customer>(ww =>
            {

            });

            modelBuilder.Entity<Team>(ww =>
            {
                ww.HasOne(ww => ww.ExecutionCompany).WithMany(ww => ww.Teams).HasForeignKey(ww => ww.ExecutionCompanyID).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<LogFile>(lf =>
            {
                lf.HasOne(lf => lf.Device).WithMany(d => d.LogFiles).HasForeignKey(lf => lf.DeviceID).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<TeamUser>(ww =>
            {
                ww.HasOne(ww => ww.Team).WithMany(ww => ww.TeamUsers).HasForeignKey(ww => ww.TeamID).OnDelete(DeleteBehavior.Cascade);
                ww.HasOne(ww => ww.ExecutionCompanyUser).WithMany(ww => ww.TeamUsers).HasForeignKey(ww => ww.ExecutionCompanyUserID).OnDelete(DeleteBehavior.Cascade);
                ww.HasIndex(ww => new { ww.TeamID, ww.ExecutionCompanyUserID }).IsUnique();
            });

            modelBuilder.Entity<Device>(ww =>
            {
                ww.HasOne(ww => ww.Team).WithMany(ww => ww.Devices).HasForeignKey(ww => ww.TeamID).OnDelete(DeleteBehavior.SetNull);
                ww.HasOne(ww => ww.LastGroup).WithOne(ww => ww.LastDevice).HasForeignKey<Device>(ww => ww.LastGroupID).OnDelete(DeleteBehavior.Cascade);
                ww.HasOne(ww => ww.Customer).WithMany(ww => ww.Devices).HasForeignKey(ww => ww.CustomerID).OnDelete(DeleteBehavior.Cascade);
                ww.HasOne(ww => ww.DeviceModel).WithMany(ww => ww.Devices).HasForeignKey(ww => ww.DeviceModelID).OnDelete(DeleteBehavior.Cascade);
                ww.HasOne(ww => ww.ExecutionCompany).WithMany(ww => ww.Devices).HasForeignKey(ww => ww.ExecutionCompanyID).OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<TelemetryGroup>(ww =>
            {
                ww.HasOne(ww => ww.Device).WithMany(ww => ww.TelemetryGroups).HasForeignKey(ww => ww.DeviceID).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CustomerUser>(ww =>
            {
                ww.HasOne(ww => ww.Customer).WithMany(ww => ww.CustomerUsers).OnDelete(DeleteBehavior.Cascade);
                ww.HasIndex(ww => new { ww.UserID }).IsUnique();
            });

            modelBuilder.Entity<ExecutionCompanyUser>(ww =>
            {
                ww.HasOne(ww => ww.ExecutionCompany).WithMany(ww => ww.ExecutionCompanyUsers).OnDelete(DeleteBehavior.Cascade);
                ww.HasIndex(ww => new { ww.UserID }).IsUnique();
            });

            modelBuilder.Entity<TelemetryRecord>(ww =>
            {
                ww.HasOne(ww => ww.Group).WithMany(ww => ww.Records).HasForeignKey(ww => ww.GroupID).OnDelete(DeleteBehavior.Cascade);
                ww.Property(ww => ww.LocationPoint).HasColumnType("geography (point)");
            });
            modelBuilder.Entity<Field>(ww =>
            {
                ww.HasOne(ww => ww.Customer).WithMany(ww => ww.Fields).HasForeignKey(ww => ww.CustomerID).OnDelete(DeleteBehavior.Cascade);
                ww.HasOne(ww => ww.ExecutionCompany).WithMany(ww => ww.Fields).HasForeignKey(ww => ww.ExecutionCompanyID).OnDelete(DeleteBehavior.Cascade);
                ww.Property(ww => ww.Border).HasColumnType("geography (polygon)");
                ww.Property(ww => ww.Flyway).HasColumnType("geography (linestring)");
                ww.Property(ww => ww.GPSPoints).HasColumnType("geography (multipoint)");
                ww.Property(ww => ww.LocationPoint).HasColumnType("geography (point)");
                ww.Property(ww => ww.CalibrationPoints).HasColumnType("geography (multipoint)");
            });

            modelBuilder.Entity<FlightStat>(ww =>
            {
                ww.HasOne(ww => ww.Device).WithMany(ww => ww.FlightStats).HasForeignKey(ww => ww.DeviceID).OnDelete(DeleteBehavior.Cascade);
                ww.HasOne(ww => ww.Customer).WithMany(ww => ww.FlightStats).HasForeignKey(ww => ww.CustomerID).OnDelete(DeleteBehavior.Cascade);
                ww.Property(ww => ww.FlywayPoints).HasColumnType("geography (linestring)");

            });

            modelBuilder.Entity<Plan>(ww =>
            {
                ww.Property(ww => ww.Location).HasColumnType("geography (point)");
                ww.HasOne(ww => ww.Device).WithMany(ww => ww.Plans).HasForeignKey(ww => ww.DeviceID).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}