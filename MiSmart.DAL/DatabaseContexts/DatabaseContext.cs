using MiSmart.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace MiSmart.DAL.DatabaseContexts
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
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
        public DbSet<Battery> Batteries { get; set; }
        public DbSet<BatteryGroupLog> BatteryGroupLogs { get; set; }
        public DbSet<BatteryLog> BatteryLogs { get; set; }
        public DbSet<BatteryModel> BatteryModels { get; set; }
        public DbSet<ExecutionCompanyUserFlightStat> ExecutionCompanyUserFlightStats { get; set; }
        public DbSet<ExecutionCompanySetting> ExecutionCompanySettings { get; set; }
        public DbSet<StreamingLink> StreamingLinks { get; set; }
        public DbSet<MaintenanceReport> MaintenanceReports { get; set; }
        public DbSet<FlightStatReportRecord> FlightStatReportRecords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("postgis");



            modelBuilder.Entity<StreamingLink>(ww =>
            {
                ww.HasOne(ww => ww.Device).WithMany(ww => ww.StreamingLinks).HasForeignKey(ww => ww.DeviceID).OnDelete(DeleteBehavior.Cascade);
            });
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
                ww.HasOne(ww => ww.LastGroup).WithOne(ww => ww.LastDevice).HasForeignKey<Device>(ww => ww.LastGroupID).OnDelete(DeleteBehavior.SetNull);
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
                ww.HasOne(ww => ww.Customer).WithMany(ww => ww.CustomerUsers).HasForeignKey(ww => ww.CustomerID).OnDelete(DeleteBehavior.Cascade);
                ww.HasIndex(ww => new { ww.UserUUID }).IsUnique();
            });

            modelBuilder.Entity<ExecutionCompanyUser>(ww =>
            {
                ww.HasOne(ww => ww.ExecutionCompany).WithMany(ww => ww.ExecutionCompanyUsers).OnDelete(DeleteBehavior.Cascade);
                ww.HasIndex(ww => new { ww.UserUUID }).IsUnique();
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
                ww.HasOne(ww => ww.ExecutionCompany).WithMany(ww => ww.FlightStats).HasForeignKey(ww => ww.ExecutionCompanyID).OnDelete(DeleteBehavior.SetNull);
                ww.HasOne(ww => ww.Team).WithMany(ww => ww.FlightStats).HasForeignKey(ww => ww.TeamID).OnDelete(DeleteBehavior.SetNull);
                ww.Property(ww => ww.FlywayPoints).HasColumnType("geography (linestring)");

            });

            modelBuilder.Entity<Plan>(ww =>
            {
                ww.Property(ww => ww.Location).HasColumnType("geography (point)");
                ww.HasOne(ww => ww.Device).WithMany(ww => ww.Plans).HasForeignKey(ww => ww.DeviceID).OnDelete(DeleteBehavior.Cascade);
                ww.Property(ww => ww.CreatedTime).HasDefaultValueSql("now() at time zone 'utc'");
            });

            modelBuilder.Entity<BatteryModel>(bm =>
            {

            });

            modelBuilder.Entity<Battery>(b =>
            {
                b.HasOne(b => b.ExecutionCompany).WithMany(c => c.Batteries).HasForeignKey(b => b.ExecutionCompanyID).OnDelete(DeleteBehavior.SetNull);
                b.HasOne(b => b.LastGroup).WithMany(g => g.LastBatteries).HasForeignKey(b => b.LastGroupID).OnDelete(DeleteBehavior.SetNull);
                b.HasOne(b => b.BatteryModel).WithMany(bm => bm.Batteries).HasForeignKey(b => b.BatteryModelID).OnDelete(DeleteBehavior.Cascade);
                b.Property(b => b.CreatedTime).HasDefaultValueSql("now() at time zone 'utc'");
            });
            modelBuilder.Entity<BatteryGroupLog>(bgl =>
            {
                bgl.HasOne(bgl => bgl.Battery).WithMany(b => b.GroupLogs).HasForeignKey(bgl => bgl.BatteryID).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<BatteryLog>(bl =>
            {
                bl.HasOne(bl => bl.GroupLog).WithMany(bgl => bgl.Logs).HasForeignKey(bl => bl.GroupLogID).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ExecutionCompanyUserFlightStat>(ww =>
            {
                ww.HasOne(ww => ww.FlightStat).WithMany(ww => ww.ExecutionCompanyUserFlightStats).HasForeignKey(ww => ww.FlightStatID).OnDelete(DeleteBehavior.Cascade);
                ww.HasOne(ww => ww.ExecutionCompanyUser).WithMany(ww => ww.ExecutionCompanyUserFlightStats).HasForeignKey(ww => ww.ExecutionCompanyUserID).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ExecutionCompanySetting>(ww =>
            {
                ww.HasOne(ww => ww.ExecutionCompany).WithMany(ww => ww.Settings).HasForeignKey(ww => ww.ExecutionCompanyID).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MaintenanceReport>(ww =>
            {
                ww.HasOne(ww => ww.Device).WithMany(ww => ww.MaintenanceReports).HasForeignKey(ww => ww.DeviceID).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<FlightStatReportRecord>(ww =>
            {
                ww.HasOne(ww => ww.FlightStat).WithMany(ww => ww.FlightStatReportRecords).HasForeignKey(ww => ww.FlightStatID).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}