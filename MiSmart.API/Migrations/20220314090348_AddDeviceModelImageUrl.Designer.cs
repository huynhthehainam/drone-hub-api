﻿// <auto-generated />
using System;
using MiSmart.DAL.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MiSmart.API.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20220314090348_AddDeviceModelImageUrl")]
    partial class AddDeviceModelImageUrl
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasPostgresExtension("postgis")
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("MiSmart.DAL.Models.Customer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.CustomerUser", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("CustomerID")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<long>("UserID")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("UserID")
                        .IsUnique();

                    b.ToTable("CustomerUsers");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Device", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("AccessToken")
                        .HasColumnType("text");

                    b.Property<int>("CustomerID")
                        .HasColumnType("integer");

                    b.Property<int>("DeviceModelID")
                        .HasColumnType("integer");

                    b.Property<Guid?>("LastGroupID")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime?>("NextGeneratingAccessTokenTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<long?>("TeamID")
                        .HasColumnType("bigint");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<Guid>("UUID")
                        .HasColumnType("uuid");

                    b.HasKey("ID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("DeviceModelID");

                    b.HasIndex("LastGroupID")
                        .IsUnique();

                    b.HasIndex("TeamID");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.DeviceModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("FileUrl")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("DeviceModels");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Field", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityByDefaultColumn();

                    b.Property<Polygon>("Border")
                        .HasColumnType("geography (polygon)");

                    b.Property<MultiPoint>("CalibrationPoints")
                        .HasColumnType("geography (multipoint)");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("CustomerID")
                        .HasColumnType("integer");

                    b.Property<double>("EdgeOffset")
                        .HasColumnType("double precision");

                    b.Property<string>("FieldLocation")
                        .HasColumnType("text");

                    b.Property<string>("FieldName")
                        .HasColumnType("text");

                    b.Property<LineString>("Flyway")
                        .HasColumnType("geography (linestring)");

                    b.Property<MultiPoint>("GPSPoints")
                        .HasColumnType("geography (multipoint)");

                    b.Property<double>("InnerArea")
                        .HasColumnType("double precision");

                    b.Property<bool>("IsLargeFarm")
                        .HasColumnType("boolean");

                    b.Property<Point>("LocationPoint")
                        .HasColumnType("geography (point)");

                    b.Property<double>("MappingArea")
                        .HasColumnType("double precision");

                    b.Property<double>("MappingTime")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("PilotName")
                        .HasColumnType("text");

                    b.Property<double>("SprayDir")
                        .HasColumnType("double precision");

                    b.Property<double>("SprayWidth")
                        .HasColumnType("double precision");

                    b.Property<DateTime?>("UpdatedTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double>("WorkArea")
                        .HasColumnType("double precision");

                    b.Property<double>("WorkSpeed")
                        .HasColumnType("double precision");

                    b.HasKey("ID");

                    b.HasIndex("CustomerID");

                    b.ToTable("Fields");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.FlightStat", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("CustomerID")
                        .HasColumnType("integer");

                    b.Property<int>("DeviceID")
                        .HasColumnType("integer");

                    b.Property<string>("DeviceName")
                        .HasColumnType("text");

                    b.Property<string>("FieldName")
                        .HasColumnType("text");

                    b.Property<double>("FlightDuration")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("FlightTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Flights")
                        .HasColumnType("integer");

                    b.Property<LineString>("FlywayPoints")
                        .HasColumnType("geography (linestring)");

                    b.Property<string>("PilotName")
                        .HasColumnType("text");

                    b.Property<double>("TaskArea")
                        .HasColumnType("double precision");

                    b.Property<string>("TaskLocation")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("DeviceID");

                    b.ToTable("FlightStats");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Plan", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("DeviceID")
                        .HasColumnType("integer");

                    b.Property<byte[]>("FileBytes")
                        .HasColumnType("bytea");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<Point>("Location")
                        .HasColumnType("geography (point)");

                    b.HasKey("ID");

                    b.HasIndex("DeviceID");

                    b.ToTable("Plans");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Team", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("CustomerID")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<double>("TotalFlightDuration")
                        .HasColumnType("double precision");

                    b.Property<long>("TotalFlights")
                        .HasColumnType("bigint");

                    b.Property<double>("TotalTaskArea")
                        .HasColumnType("double precision");

                    b.HasKey("ID");

                    b.HasIndex("CustomerID");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.TeamUser", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityByDefaultColumn();

                    b.Property<long>("CustomerUserID")
                        .HasColumnType("bigint");

                    b.Property<long>("TeamID")
                        .HasColumnType("bigint");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("CustomerUserID");

                    b.HasIndex("TeamID", "CustomerUserID")
                        .IsUnique();

                    b.ToTable("TeamUsers");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.TelemetryGroup", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("DeviceID")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("DeviceID");

                    b.ToTable("TelemetryGroups");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.TelemetryRecord", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AdditionalInformationString")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double>("Direction")
                        .HasColumnType("double precision");

                    b.Property<Guid>("GroupID")
                        .HasColumnType("uuid");

                    b.Property<Point>("LocationPoint")
                        .HasColumnType("geography (point)");

                    b.HasKey("ID");

                    b.HasIndex("GroupID");

                    b.ToTable("TelemetryRecords");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.CustomerUser", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.Customer", "Customer")
                        .WithMany("CustomerUsers")
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Device", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.Customer", "Customer")
                        .WithMany("Devices")
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MiSmart.DAL.Models.DeviceModel", "DeviceModel")
                        .WithMany("Devices")
                        .HasForeignKey("DeviceModelID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MiSmart.DAL.Models.TelemetryGroup", "LastGroup")
                        .WithOne("LastDevice")
                        .HasForeignKey("MiSmart.DAL.Models.Device", "LastGroupID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MiSmart.DAL.Models.Team", "Team")
                        .WithMany("Devices")
                        .HasForeignKey("TeamID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Customer");

                    b.Navigation("DeviceModel");

                    b.Navigation("LastGroup");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Field", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.Customer", "Customer")
                        .WithMany("Fields")
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.FlightStat", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.Customer", "Customer")
                        .WithMany("FlightStats")
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MiSmart.DAL.Models.Device", "Device")
                        .WithMany("FlightStats")
                        .HasForeignKey("DeviceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Device");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Plan", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.Device", "Device")
                        .WithMany("Plans")
                        .HasForeignKey("DeviceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Team", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.Customer", "Customer")
                        .WithMany("Teams")
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.TeamUser", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.CustomerUser", "CustomerUser")
                        .WithMany("TeamUsers")
                        .HasForeignKey("CustomerUserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MiSmart.DAL.Models.Team", "Team")
                        .WithMany("TeamUsers")
                        .HasForeignKey("TeamID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CustomerUser");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.TelemetryGroup", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.Device", "Device")
                        .WithMany("TelemetryGroups")
                        .HasForeignKey("DeviceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.TelemetryRecord", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.TelemetryGroup", "Group")
                        .WithMany("Records")
                        .HasForeignKey("GroupID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Customer", b =>
                {
                    b.Navigation("CustomerUsers");

                    b.Navigation("Devices");

                    b.Navigation("Fields");

                    b.Navigation("FlightStats");

                    b.Navigation("Teams");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.CustomerUser", b =>
                {
                    b.Navigation("TeamUsers");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Device", b =>
                {
                    b.Navigation("FlightStats");

                    b.Navigation("Plans");

                    b.Navigation("TelemetryGroups");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.DeviceModel", b =>
                {
                    b.Navigation("Devices");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Team", b =>
                {
                    b.Navigation("Devices");

                    b.Navigation("TeamUsers");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.TelemetryGroup", b =>
                {
                    b.Navigation("LastDevice");

                    b.Navigation("Records");
                });
#pragma warning restore 612, 618
        }
    }
}
