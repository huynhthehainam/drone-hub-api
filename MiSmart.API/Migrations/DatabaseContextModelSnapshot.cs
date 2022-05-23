﻿// <auto-generated />
using System;
using System.Collections.Generic;
using MiSmart.DAL.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MiSmart.API.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasPostgresExtension("postgis")
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("MiSmart.DAL.Models.Battery", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("ActualID")
                        .HasColumnType("text");

                    b.Property<int>("BatteryModelID")
                        .HasColumnType("integer");

                    b.Property<int?>("ExecutionCompanyID")
                        .HasColumnType("integer");

                    b.Property<Guid?>("LastGroupID")
                        .HasColumnType("uuid");

                    b.HasKey("ID");

                    b.HasIndex("BatteryModelID");

                    b.HasIndex("ExecutionCompanyID");

                    b.HasIndex("LastGroupID");

                    b.ToTable("Batteries");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.BatteryGroupLog", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("BatteryID")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("ID");

                    b.HasIndex("BatteryID");

                    b.ToTable("BatteryGroupLogs");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.BatteryLog", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("CellMaximumVoltage")
                        .HasColumnType("double precision");

                    b.Property<string>("CellMaximumVoltageUnit")
                        .HasColumnType("text");

                    b.Property<double>("CellMinimumVoltage")
                        .HasColumnType("double precision");

                    b.Property<string>("CellMinimumVoltageUnit")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double>("Current")
                        .HasColumnType("double precision");

                    b.Property<string>("CurrentUnit")
                        .HasColumnType("text");

                    b.Property<int>("CycleCount")
                        .HasColumnType("integer");

                    b.Property<Guid>("GroupLogID")
                        .HasColumnType("uuid");

                    b.Property<double>("PercentRemaining")
                        .HasColumnType("double precision");

                    b.Property<double>("Temperature")
                        .HasColumnType("double precision");

                    b.Property<string>("TemperatureUnit")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("GroupLogID");

                    b.ToTable("BatteryLogs");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.BatteryModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("FileUrl")
                        .HasColumnType("text");

                    b.Property<string>("ManufacturerName")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("BatteryModels");
                });

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

                    b.Property<int?>("ExecutionCompanyID")
                        .HasColumnType("integer");

                    b.Property<string>("LastBatterGroupLogsString")
                        .HasColumnType("text");

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

                    b.HasIndex("ExecutionCompanyID");

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

            modelBuilder.Entity("MiSmart.DAL.Models.ExecutionCompany", b =>
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

                    b.ToTable("ExecutionCompanies");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.ExecutionCompanySetting", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<double>("CostPerHectare")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("ExecutionCompanyID")
                        .HasColumnType("integer");

                    b.Property<double>("MainPilotCostPerHectare")
                        .HasColumnType("double precision");

                    b.Property<double>("SubPilotCostPerHectare")
                        .HasColumnType("double precision");

                    b.HasKey("ID");

                    b.HasIndex("ExecutionCompanyID");

                    b.ToTable("ExecutionCompanySettings");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.ExecutionCompanyUser", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("ExecutionCompanyID")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<long>("UserID")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("ExecutionCompanyID");

                    b.HasIndex("UserID")
                        .IsUnique();

                    b.ToTable("ExecutionCompanyUsers");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.ExecutionCompanyUserFlightStat", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("ExecutionCompanyUserID")
                        .HasColumnType("bigint");

                    b.Property<Guid>("FlightStatID")
                        .HasColumnType("uuid");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("ExecutionCompanyUserID");

                    b.HasIndex("FlightStatID");

                    b.ToTable("ExecutionCompanyUserFlightStats");
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

                    b.Property<int>("ExecutionCompanyID")
                        .HasColumnType("integer");

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

                    b.HasIndex("ExecutionCompanyID");

                    b.ToTable("Fields");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.FlightStat", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("Cost")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("CustomerID")
                        .HasColumnType("integer");

                    b.Property<int>("DeviceID")
                        .HasColumnType("integer");

                    b.Property<string>("DeviceName")
                        .HasColumnType("text");

                    b.Property<int?>("ExecutionCompanyID")
                        .HasColumnType("integer");

                    b.Property<string>("FieldName")
                        .HasColumnType("text");

                    b.Property<double>("FlightDuration")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("FlightTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("FlightUID")
                        .HasColumnType("uuid");

                    b.Property<int>("Flights")
                        .HasColumnType("integer");

                    b.Property<LineString>("FlywayPoints")
                        .HasColumnType("geography (linestring)");

                    b.Property<string>("MedicinesString")
                        .HasColumnType("text");

                    b.Property<int>("Mode")
                        .HasColumnType("integer");

                    b.Property<string>("PilotName")
                        .HasColumnType("text");

                    b.Property<List<int>>("SprayedIndexes")
                        .HasColumnType("integer[]");

                    b.Property<string>("TMUserString")
                        .HasColumnType("text");

                    b.Property<string>("TMUserUID")
                        .HasColumnType("text");

                    b.Property<double>("TaskArea")
                        .HasColumnType("double precision");

                    b.Property<string>("TaskLocation")
                        .HasColumnType("text");

                    b.Property<long?>("TeamID")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("DeviceID");

                    b.HasIndex("ExecutionCompanyID");

                    b.HasIndex("TeamID");

                    b.ToTable("FlightStats");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.LogFile", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("DeviceID")
                        .HasColumnType("integer");

                    b.Property<string>("FileUrl")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("DeviceID");

                    b.ToTable("LogFiles");
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

            modelBuilder.Entity("MiSmart.DAL.Models.StreamingLink", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("DeviceID")
                        .HasColumnType("integer");

                    b.Property<string>("Link")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("DeviceID");

                    b.ToTable("StreamingLinks");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Team", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("ExecutionCompanyID")
                        .HasColumnType("integer");

                    b.Property<bool>("IsDisbanded")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<double>("TotalFlightDuration")
                        .HasColumnType("double precision");

                    b.Property<long>("TotalFlights")
                        .HasColumnType("bigint");

                    b.Property<double>("TotalTaskArea")
                        .HasColumnType("double precision");

                    b.HasKey("ID");

                    b.HasIndex("ExecutionCompanyID");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.TeamUser", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityByDefaultColumn();

                    b.Property<long>("ExecutionCompanyUserID")
                        .HasColumnType("bigint");

                    b.Property<long>("TeamID")
                        .HasColumnType("bigint");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("ExecutionCompanyUserID");

                    b.HasIndex("TeamID", "ExecutionCompanyUserID")
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

            modelBuilder.Entity("MiSmart.DAL.Models.Battery", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.BatteryModel", "BatteryModel")
                        .WithMany("Batteries")
                        .HasForeignKey("BatteryModelID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MiSmart.DAL.Models.ExecutionCompany", "ExecutionCompany")
                        .WithMany("Batteries")
                        .HasForeignKey("ExecutionCompanyID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("MiSmart.DAL.Models.BatteryGroupLog", "LastGroup")
                        .WithMany("LastBatteries")
                        .HasForeignKey("LastGroupID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("BatteryModel");

                    b.Navigation("ExecutionCompany");

                    b.Navigation("LastGroup");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.BatteryGroupLog", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.Battery", "Battery")
                        .WithMany("GroupLogs")
                        .HasForeignKey("BatteryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Battery");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.BatteryLog", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.BatteryGroupLog", "GroupLog")
                        .WithMany("Logs")
                        .HasForeignKey("GroupLogID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GroupLog");
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

                    b.HasOne("MiSmart.DAL.Models.ExecutionCompany", "ExecutionCompany")
                        .WithMany("Devices")
                        .HasForeignKey("ExecutionCompanyID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("MiSmart.DAL.Models.TelemetryGroup", "LastGroup")
                        .WithOne("LastDevice")
                        .HasForeignKey("MiSmart.DAL.Models.Device", "LastGroupID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("MiSmart.DAL.Models.Team", "Team")
                        .WithMany("Devices")
                        .HasForeignKey("TeamID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Customer");

                    b.Navigation("DeviceModel");

                    b.Navigation("ExecutionCompany");

                    b.Navigation("LastGroup");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.ExecutionCompanySetting", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.ExecutionCompany", "ExecutionCompany")
                        .WithMany("Settings")
                        .HasForeignKey("ExecutionCompanyID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ExecutionCompany");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.ExecutionCompanyUser", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.ExecutionCompany", "ExecutionCompany")
                        .WithMany("ExecutionCompanyUsers")
                        .HasForeignKey("ExecutionCompanyID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ExecutionCompany");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.ExecutionCompanyUserFlightStat", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.ExecutionCompanyUser", "ExecutionCompanyUser")
                        .WithMany("ExecutionCompanyUserFlightStats")
                        .HasForeignKey("ExecutionCompanyUserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MiSmart.DAL.Models.FlightStat", "FlightStat")
                        .WithMany("ExecutionCompanyUserFlightStats")
                        .HasForeignKey("FlightStatID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ExecutionCompanyUser");

                    b.Navigation("FlightStat");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Field", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.Customer", "Customer")
                        .WithMany("Fields")
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MiSmart.DAL.Models.ExecutionCompany", "ExecutionCompany")
                        .WithMany("Fields")
                        .HasForeignKey("ExecutionCompanyID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("ExecutionCompany");
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

                    b.HasOne("MiSmart.DAL.Models.ExecutionCompany", "ExecutionCompany")
                        .WithMany("FlightStats")
                        .HasForeignKey("ExecutionCompanyID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("MiSmart.DAL.Models.Team", "Team")
                        .WithMany("FlightStats")
                        .HasForeignKey("TeamID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Customer");

                    b.Navigation("Device");

                    b.Navigation("ExecutionCompany");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.LogFile", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.Device", "Device")
                        .WithMany("LogFiles")
                        .HasForeignKey("DeviceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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

            modelBuilder.Entity("MiSmart.DAL.Models.StreamingLink", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.Device", "Device")
                        .WithMany("StreamingLinks")
                        .HasForeignKey("DeviceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Team", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.ExecutionCompany", "ExecutionCompany")
                        .WithMany("Teams")
                        .HasForeignKey("ExecutionCompanyID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ExecutionCompany");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.TeamUser", b =>
                {
                    b.HasOne("MiSmart.DAL.Models.ExecutionCompanyUser", "ExecutionCompanyUser")
                        .WithMany("TeamUsers")
                        .HasForeignKey("ExecutionCompanyUserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MiSmart.DAL.Models.Team", "Team")
                        .WithMany("TeamUsers")
                        .HasForeignKey("TeamID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ExecutionCompanyUser");

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

            modelBuilder.Entity("MiSmart.DAL.Models.Battery", b =>
                {
                    b.Navigation("GroupLogs");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.BatteryGroupLog", b =>
                {
                    b.Navigation("LastBatteries");

                    b.Navigation("Logs");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.BatteryModel", b =>
                {
                    b.Navigation("Batteries");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Customer", b =>
                {
                    b.Navigation("CustomerUsers");

                    b.Navigation("Devices");

                    b.Navigation("Fields");

                    b.Navigation("FlightStats");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Device", b =>
                {
                    b.Navigation("FlightStats");

                    b.Navigation("LogFiles");

                    b.Navigation("Plans");

                    b.Navigation("StreamingLinks");

                    b.Navigation("TelemetryGroups");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.DeviceModel", b =>
                {
                    b.Navigation("Devices");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.ExecutionCompany", b =>
                {
                    b.Navigation("Batteries");

                    b.Navigation("Devices");

                    b.Navigation("ExecutionCompanyUsers");

                    b.Navigation("Fields");

                    b.Navigation("FlightStats");

                    b.Navigation("Settings");

                    b.Navigation("Teams");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.ExecutionCompanyUser", b =>
                {
                    b.Navigation("ExecutionCompanyUserFlightStats");

                    b.Navigation("TeamUsers");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.FlightStat", b =>
                {
                    b.Navigation("ExecutionCompanyUserFlightStats");
                });

            modelBuilder.Entity("MiSmart.DAL.Models.Team", b =>
                {
                    b.Navigation("Devices");

                    b.Navigation("FlightStats");

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
