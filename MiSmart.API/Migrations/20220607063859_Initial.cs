using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "BatteryModels",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ManufacturerName = table.Column<string>(type: "text", nullable: true),
                    FileUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryModels", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DeviceModels",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    FileUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceModels", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ExecutionCompanies",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionCompanies", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CustomerUsers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserUUID = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerUsers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomerUsers_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExecutionCompanySettings",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MainPilotCostPerHectare = table.Column<double>(type: "double precision", nullable: false),
                    SubPilotCostPerHectare = table.Column<double>(type: "double precision", nullable: false),
                    CostPerHectare = table.Column<double>(type: "double precision", nullable: false),
                    ExecutionCompanyID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionCompanySettings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ExecutionCompanySettings_ExecutionCompanies_ExecutionCompan~",
                        column: x => x.ExecutionCompanyID,
                        principalTable: "ExecutionCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExecutionCompanyUsers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserUUID = table.Column<Guid>(type: "uuid", nullable: false),
                    ExecutionCompanyID = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionCompanyUsers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ExecutionCompanyUsers_ExecutionCompanies_ExecutionCompanyID",
                        column: x => x.ExecutionCompanyID,
                        principalTable: "ExecutionCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FieldName = table.Column<string>(type: "text", nullable: true),
                    FieldLocation = table.Column<string>(type: "text", nullable: true),
                    PilotName = table.Column<string>(type: "text", nullable: true),
                    MappingArea = table.Column<double>(type: "double precision", nullable: false),
                    MappingTime = table.Column<double>(type: "double precision", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CustomerID = table.Column<int>(type: "integer", nullable: false),
                    ExecutionCompanyID = table.Column<int>(type: "integer", nullable: false),
                    Border = table.Column<Polygon>(type: "geography (polygon)", nullable: true),
                    Flyway = table.Column<LineString>(type: "geography (linestring)", nullable: true),
                    GPSPoints = table.Column<MultiPoint>(type: "geography (multipoint)", nullable: true),
                    LocationPoint = table.Column<Point>(type: "geography (point)", nullable: true),
                    CalibrationPoints = table.Column<MultiPoint>(type: "geography (multipoint)", nullable: true),
                    WorkSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WorkArea = table.Column<double>(type: "double precision", nullable: false),
                    InnerArea = table.Column<double>(type: "double precision", nullable: false),
                    SprayWidth = table.Column<double>(type: "double precision", nullable: false),
                    SprayDir = table.Column<double>(type: "double precision", nullable: false),
                    IsLargeFarm = table.Column<bool>(type: "boolean", nullable: false),
                    EdgeOffset = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Fields_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fields_ExecutionCompanies_ExecutionCompanyID",
                        column: x => x.ExecutionCompanyID,
                        principalTable: "ExecutionCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    TotalTaskArea = table.Column<double>(type: "double precision", nullable: false),
                    TotalFlightDuration = table.Column<double>(type: "double precision", nullable: false),
                    TotalFlights = table.Column<long>(type: "bigint", nullable: false),
                    ExecutionCompanyID = table.Column<int>(type: "integer", nullable: false),
                    IsDisbanded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Teams_ExecutionCompanies_ExecutionCompanyID",
                        column: x => x.ExecutionCompanyID,
                        principalTable: "ExecutionCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamUsers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeamID = table.Column<long>(type: "bigint", nullable: false),
                    ExecutionCompanyUserID = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamUsers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TeamUsers_ExecutionCompanyUsers_ExecutionCompanyUserID",
                        column: x => x.ExecutionCompanyUserID,
                        principalTable: "ExecutionCompanyUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamUsers_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Batteries",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActualID = table.Column<string>(type: "text", nullable: true),
                    BatteryModelID = table.Column<int>(type: "integer", nullable: false),
                    LastGroupID = table.Column<Guid>(type: "uuid", nullable: true),
                    ExecutionCompanyID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batteries", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Batteries_BatteryModels_BatteryModelID",
                        column: x => x.BatteryModelID,
                        principalTable: "BatteryModels",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Batteries_ExecutionCompanies_ExecutionCompanyID",
                        column: x => x.ExecutionCompanyID,
                        principalTable: "ExecutionCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BatteryGroupLogs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    BatteryID = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryGroupLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BatteryGroupLogs_Batteries_BatteryID",
                        column: x => x.BatteryID,
                        principalTable: "Batteries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatteryLogs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupLogID = table.Column<Guid>(type: "uuid", nullable: false),
                    PercentRemaining = table.Column<double>(type: "double precision", nullable: false),
                    Temperature = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureUnit = table.Column<string>(type: "text", nullable: true),
                    CellMinimumVoltage = table.Column<double>(type: "double precision", nullable: false),
                    CellMinimumVoltageUnit = table.Column<string>(type: "text", nullable: true),
                    CellMaximumVoltage = table.Column<double>(type: "double precision", nullable: false),
                    CellMaximumVoltageUnit = table.Column<string>(type: "text", nullable: true),
                    CycleCount = table.Column<int>(type: "integer", nullable: false),
                    Current = table.Column<double>(type: "double precision", nullable: false),
                    CurrentUnit = table.Column<string>(type: "text", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BatteryLogs_BatteryGroupLogs_GroupLogID",
                        column: x => x.GroupLogID,
                        principalTable: "BatteryGroupLogs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UUID = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TeamID = table.Column<long>(type: "bigint", nullable: true),
                    CustomerID = table.Column<int>(type: "integer", nullable: false),
                    ExecutionCompanyID = table.Column<int>(type: "integer", nullable: true),
                    AccessToken = table.Column<string>(type: "text", nullable: true),
                    NextGeneratingAccessTokenTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeviceModelID = table.Column<int>(type: "integer", nullable: false),
                    LastGroupID = table.Column<Guid>(type: "uuid", nullable: true),
                    LastBatterGroupLogs = table.Column<List<Guid>>(type: "uuid[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Devices_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Devices_DeviceModels_DeviceModelID",
                        column: x => x.DeviceModelID,
                        principalTable: "DeviceModels",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Devices_ExecutionCompanies_ExecutionCompanyID",
                        column: x => x.ExecutionCompanyID,
                        principalTable: "ExecutionCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Devices_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FlightStats",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FlightTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TaskLocation = table.Column<string>(type: "text", nullable: true),
                    Flights = table.Column<int>(type: "integer", nullable: false),
                    FieldName = table.Column<string>(type: "text", nullable: true),
                    DeviceName = table.Column<string>(type: "text", nullable: true),
                    TaskArea = table.Column<double>(type: "double precision", nullable: false),
                    FlightDuration = table.Column<double>(type: "double precision", nullable: false),
                    PilotName = table.Column<string>(type: "text", nullable: true),
                    TMUserUID = table.Column<string>(type: "text", nullable: true),
                    TMUser = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    FlywayPoints = table.Column<LineString>(type: "geography (linestring)", nullable: true),
                    SprayedIndexes = table.Column<List<int>>(type: "integer[]", nullable: true),
                    DeviceID = table.Column<int>(type: "integer", nullable: false),
                    FlightUID = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerID = table.Column<int>(type: "integer", nullable: false),
                    TeamID = table.Column<long>(type: "bigint", nullable: true),
                    ExecutionCompanyID = table.Column<int>(type: "integer", nullable: true),
                    Mode = table.Column<int>(type: "integer", nullable: false),
                    Cost = table.Column<double>(type: "double precision", nullable: false),
                    MedicinesString = table.Column<string>(type: "text", nullable: true),
                    Medicines = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    AdditionalInformation = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    GCSVersion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightStats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FlightStats_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightStats_Devices_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightStats_ExecutionCompanies_ExecutionCompanyID",
                        column: x => x.ExecutionCompanyID,
                        principalTable: "ExecutionCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FlightStats_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LogFiles",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: true),
                    DeviceID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogFiles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LogFiles_Devices_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Location = table.Column<Point>(type: "geography (point)", nullable: true),
                    FileBytes = table.Column<byte[]>(type: "bytea", nullable: true),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    DeviceID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Plans_Devices_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StreamingLinks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceID = table.Column<int>(type: "integer", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamingLinks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StreamingLinks_Devices_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelemetryGroups",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceID = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetryGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TelemetryGroups_Devices_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExecutionCompanyUserFlightStats",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    ExecutionCompanyUserID = table.Column<long>(type: "bigint", nullable: false),
                    FlightStatID = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionCompanyUserFlightStats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ExecutionCompanyUserFlightStats_ExecutionCompanyUsers_Execu~",
                        column: x => x.ExecutionCompanyUserID,
                        principalTable: "ExecutionCompanyUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExecutionCompanyUserFlightStats_FlightStats_FlightStatID",
                        column: x => x.FlightStatID,
                        principalTable: "FlightStats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelemetryRecords",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LocationPoint = table.Column<Point>(type: "geography (point)", nullable: true),
                    Direction = table.Column<double>(type: "double precision", nullable: false),
                    AdditionalInformation = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    GroupID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetryRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TelemetryRecords_TelemetryGroups_GroupID",
                        column: x => x.GroupID,
                        principalTable: "TelemetryGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Batteries_BatteryModelID",
                table: "Batteries",
                column: "BatteryModelID");

            migrationBuilder.CreateIndex(
                name: "IX_Batteries_ExecutionCompanyID",
                table: "Batteries",
                column: "ExecutionCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_Batteries_LastGroupID",
                table: "Batteries",
                column: "LastGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryGroupLogs_BatteryID",
                table: "BatteryGroupLogs",
                column: "BatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryLogs_GroupLogID",
                table: "BatteryLogs",
                column: "GroupLogID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerUsers_CustomerID",
                table: "CustomerUsers",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerUsers_UserUUID",
                table: "CustomerUsers",
                column: "UserUUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_CustomerID",
                table: "Devices",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceModelID",
                table: "Devices",
                column: "DeviceModelID");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ExecutionCompanyID",
                table: "Devices",
                column: "ExecutionCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_LastGroupID",
                table: "Devices",
                column: "LastGroupID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_TeamID",
                table: "Devices",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionCompanySettings_ExecutionCompanyID",
                table: "ExecutionCompanySettings",
                column: "ExecutionCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionCompanyUserFlightStats_ExecutionCompanyUserID",
                table: "ExecutionCompanyUserFlightStats",
                column: "ExecutionCompanyUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionCompanyUserFlightStats_FlightStatID",
                table: "ExecutionCompanyUserFlightStats",
                column: "FlightStatID");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionCompanyUsers_ExecutionCompanyID",
                table: "ExecutionCompanyUsers",
                column: "ExecutionCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionCompanyUsers_UserUUID",
                table: "ExecutionCompanyUsers",
                column: "UserUUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fields_CustomerID",
                table: "Fields",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_ExecutionCompanyID",
                table: "Fields",
                column: "ExecutionCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_FlightStats_CustomerID",
                table: "FlightStats",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_FlightStats_DeviceID",
                table: "FlightStats",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_FlightStats_ExecutionCompanyID",
                table: "FlightStats",
                column: "ExecutionCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_FlightStats_TeamID",
                table: "FlightStats",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_LogFiles_DeviceID",
                table: "LogFiles",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_DeviceID",
                table: "Plans",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_StreamingLinks_DeviceID",
                table: "StreamingLinks",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_ExecutionCompanyID",
                table: "Teams",
                column: "ExecutionCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_TeamUsers_ExecutionCompanyUserID",
                table: "TeamUsers",
                column: "ExecutionCompanyUserID");

            migrationBuilder.CreateIndex(
                name: "IX_TeamUsers_TeamID_ExecutionCompanyUserID",
                table: "TeamUsers",
                columns: new[] { "TeamID", "ExecutionCompanyUserID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryGroups_DeviceID",
                table: "TelemetryGroups",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryRecords_GroupID",
                table: "TelemetryRecords",
                column: "GroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_Batteries_BatteryGroupLogs_LastGroupID",
                table: "Batteries",
                column: "LastGroupID",
                principalTable: "BatteryGroupLogs",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_TelemetryGroups_LastGroupID",
                table: "Devices",
                column: "LastGroupID",
                principalTable: "TelemetryGroups",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batteries_BatteryGroupLogs_LastGroupID",
                table: "Batteries");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_ExecutionCompanies_ExecutionCompanyID",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_ExecutionCompanies_ExecutionCompanyID",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Customers_CustomerID",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceModels_DeviceModelID",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Teams_TeamID",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_TelemetryGroups_LastGroupID",
                table: "Devices");

            migrationBuilder.DropTable(
                name: "BatteryLogs");

            migrationBuilder.DropTable(
                name: "CustomerUsers");

            migrationBuilder.DropTable(
                name: "ExecutionCompanySettings");

            migrationBuilder.DropTable(
                name: "ExecutionCompanyUserFlightStats");

            migrationBuilder.DropTable(
                name: "Fields");

            migrationBuilder.DropTable(
                name: "LogFiles");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropTable(
                name: "StreamingLinks");

            migrationBuilder.DropTable(
                name: "TeamUsers");

            migrationBuilder.DropTable(
                name: "TelemetryRecords");

            migrationBuilder.DropTable(
                name: "FlightStats");

            migrationBuilder.DropTable(
                name: "ExecutionCompanyUsers");

            migrationBuilder.DropTable(
                name: "BatteryGroupLogs");

            migrationBuilder.DropTable(
                name: "Batteries");

            migrationBuilder.DropTable(
                name: "BatteryModels");

            migrationBuilder.DropTable(
                name: "ExecutionCompanies");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "DeviceModels");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "TelemetryGroups");

            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
