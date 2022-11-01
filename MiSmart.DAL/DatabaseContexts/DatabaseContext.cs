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

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<TelemetryRecord> TelemetryRecords => Set<TelemetryRecord>();
        public DbSet<TeamUser> TeamUsers => Set<TeamUser>();
        public DbSet<Device> Devices => Set<Device>();
        public DbSet<Field> Fields => Set<Field>();
        public DbSet<FlightStat> FlightStats => Set<FlightStat>();
        public DbSet<DeviceModel> DeviceModels => Set<DeviceModel>();
        public DbSet<CustomerUser> CustomerUsers => Set<CustomerUser>();
        public DbSet<Plan> Plans => Set<Plan>();
        public DbSet<TelemetryGroup> TelemetryGroups => Set<TelemetryGroup>();
        public DbSet<LogFile> LogFiles => Set<LogFile>();
        public DbSet<ExecutionCompany> ExecutionCompanies => Set<ExecutionCompany>();
        public DbSet<ExecutionCompanyUser> ExecutionCompanyUsers => Set<ExecutionCompanyUser>();
        public DbSet<Battery> Batteries => Set<Battery>();
        public DbSet<BatteryGroupLog> BatteryGroupLogs => Set<BatteryGroupLog>();
        public DbSet<BatteryLog> BatteryLogs => Set<BatteryLog>();
        public DbSet<BatteryModel> BatteryModels => Set<BatteryModel>();
        public DbSet<ExecutionCompanyUserFlightStat> ExecutionCompanyUserFlightStats => Set<ExecutionCompanyUserFlightStat>();
        public DbSet<ExecutionCompanySetting> ExecutionCompanySettings => Set<ExecutionCompanySetting>();
        public DbSet<StreamingLink> StreamingLinks => Set<StreamingLink>();
        public DbSet<MaintenanceReport> MaintenanceReports => Set<MaintenanceReport>();
        public DbSet<FlightStatReportRecord> FlightStatReportRecords => Set<FlightStatReportRecord>();
        public DbSet<LogDetail> LogDetails => Set<LogDetail>();
        public DbSet<LogReport> LogReports => Set<LogReport>();
        public DbSet<LogReportResult> LogReportResults => Set<LogReportResult>();
        public DbSet<LogToken> LogTokens => Set<LogToken>();
        public DbSet<Part> Parts => Set<Part>();
        public DbSet<LogResultDetail> LogResultDetails => Set<LogResultDetail>();
        public DbSet<SecondLogReport> SecondLogReports => Set<SecondLogReport>();

        public DbSet<DeviceModelParam> DeviceModelParams => Set<DeviceModelParam>();
        public DbSet<DeviceModelParamDetail> DeviceModelParamDetails => Set<DeviceModelParamDetail>();
        public DbSet<DeviceModelParamCentrifugalDetail> DeviceModelParamCentrifugalDetails => Set<DeviceModelParamCentrifugalDetail>();
        public DbSet<DeviceModelParamCentrifugal4Detail> DeviceModelParamCentrifugal4Details => Set<DeviceModelParamCentrifugal4Detail>();
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
                ww.HasOne(ww => ww.Battery).WithMany(ww => ww.FlightStats).HasForeignKey(ww => ww.BatteryID).OnDelete(DeleteBehavior.SetNull);
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

            modelBuilder.Entity<LogDetail>(ww =>
            {
                ww.HasOne(ww => ww.LogFile).WithOne(ww => ww.LogDetail).HasForeignKey<LogDetail>(ww => ww.LogFileID).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<LogReport>(ww =>
            {
                ww.HasOne(ww => ww.LogFile).WithOne(ww => ww.LogReport).HasForeignKey<LogReport>(ww => ww.LogFileID).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<LogReportResult>(ww =>
            {
                ww.HasOne(ww => ww.LogFile).WithOne(ww => ww.LogReportResult).HasForeignKey<LogReportResult>(ww => ww.LogFileID).OnDelete(DeleteBehavior.Cascade);
                ww.HasOne(ww => ww.ExecutionCompany).WithMany(ww => ww.LogReportResults).HasForeignKey(ww => ww.ExecutionCompanyID).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<LogResultDetail>(ww =>
            {
                ww.HasOne(ww => ww.PartError).WithMany(ww => ww.LogResultDetails).HasForeignKey(ww => ww.PartErrorID).OnDelete(DeleteBehavior.Cascade);
                ww.HasOne(ww => ww.LogReportResult).WithMany(ww => ww.LogResultDetails).HasForeignKey(ww => ww.LogReportResultID).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<LogToken>(ww =>
            {
                ww.HasOne(ww => ww.LogFile).WithMany(ww => ww.LogTokens).HasForeignKey(ww => ww.LogFileID).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<SecondLogReport>(ww =>
            {
                ww.HasOne(ww => ww.LogFile).WithOne(ww => ww.SecondLogReport).HasForeignKey<SecondLogReport>(ww => ww.LogFileID).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DeviceModelParam>(ww =>
            {
                ww.HasOne(ww => ww.DeviceModel).WithMany(ww => ww.ModelParams).HasForeignKey(ww => ww.DeviceModelID).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<DeviceModelParamDetail>(ww =>
            {
                ww.HasOne(ww => ww.DeviceModelParam).WithMany(ww => ww.Details).HasForeignKey(ww => ww.DeviceModelParamID).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<DeviceModelParamCentrifugalDetail>(ww =>
            {
                ww.HasOne(ww => ww.DeviceModelParam).WithMany(ww => ww.CentrifugalDetails).HasForeignKey(ww => ww.DeviceModelParamID).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<DeviceModelParamCentrifugal4Detail>(ww =>
            {
                ww.HasOne(ww => ww.DeviceModelParam).WithMany(ww => ww.Centrifugal4Details).HasForeignKey(ww => ww.DeviceModelParamID).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}