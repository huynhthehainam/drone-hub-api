using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Helpers;
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("postgis");
            modelBuilder.Entity<Customer>(ww =>
            {

            });
            modelBuilder.Entity<Team>(ww =>
            {
                ww.HasOne(ww => ww.Customer).WithMany(ww => ww.Teams).HasForeignKey(ww => ww.CustomerID).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TeamUser>(ww =>
            {
                ww.HasOne(ww => ww.Team).WithMany(ww => ww.TeamUsers).HasForeignKey(ww => ww.TeamID).OnDelete(DeleteBehavior.Cascade);
                ww.HasIndex(ww => new { ww.TeamID, ww.UserID }).IsUnique();
            });

            modelBuilder.Entity<Device>(ww =>
            {
                ww.HasOne(ww => ww.Team).WithMany(ww => ww.Devices).HasForeignKey(ww => ww.TeamID).OnDelete(DeleteBehavior.Cascade);
                ww.Property(ww => ww.LastPoint).HasColumnType("geography (point)");
                ww.HasOne(ww => ww.Customer).WithMany(ww => ww.Devices).HasForeignKey(ww => ww.CustomerID).OnDelete(DeleteBehavior.Cascade);
                ww.HasOne(ww => ww.DeviceModel).WithMany(ww => ww.Devices).HasForeignKey(ww => ww.DeviceModelID).OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<CustomerUser>(ww =>
            {
                ww.HasOne(ww => ww.Customer).WithMany(ww => ww.CustomerUsers).OnDelete(DeleteBehavior.Cascade);
                ww.HasIndex(ww => new { ww.CustomerID, ww.UserID }).IsUnique();
            });

            modelBuilder.Entity<TelemetryRecord>(ww =>
            {
                ww.HasOne(ww => ww.Device).WithMany(ww => ww.Records).HasForeignKey(ww => ww.DeviceID).OnDelete(DeleteBehavior.Cascade);
                ww.Property(ww => ww.LocationPoint).HasColumnType("geography (point)");


            });
            modelBuilder.Entity<Field>(ww =>
            {
                ww.HasOne(ww => ww.Customer).WithMany(ww => ww.Fields).HasForeignKey(ww => ww.CustomerID).OnDelete(DeleteBehavior.Cascade);
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
            });
        }
    }
}