﻿using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AddLogDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DroneStatus",
                table: "LogFiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string[]>(
                name: "Errors",
                table: "LogFiles",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "LogFiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LogDetails",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogFileID = table.Column<Guid>(type: "uuid", nullable: false),
                    FlightDuration = table.Column<double>(type: "double precision", nullable: false),
                    Vibe = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    PercentBattery = table.Column<double>(type: "double precision", nullable: false),
                    PercentFuel = table.Column<double>(type: "double precision", nullable: false),
                    Edge = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    BatteryCellDeviation = table.Column<double>(type: "double precision", nullable: false),
                    Area = table.Column<double>(type: "double precision", nullable: false),
                    FlySpeed = table.Column<double>(type: "double precision", nullable: false),
                    Heigh = table.Column<double>(type: "double precision", nullable: false),
                    Accel = table.Column<JsonDocument>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LogDetails_LogFiles_LogFileID",
                        column: x => x.LogFileID,
                        principalTable: "LogFiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogReportResults",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogFileID = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrls = table.Column<string[]>(type: "text[]", nullable: true),
                    Suggest = table.Column<string>(type: "text", nullable: true),
                    DetailedAnalysis = table.Column<string>(type: "text", nullable: true),
                    AnalystUUID = table.Column<Guid>(type: "uuid", nullable: false),
                    ApproverUUID = table.Column<Guid>(type: "uuid", nullable: false),
                    ExecutionCompanyID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogReportResults", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LogReportResults_ExecutionCompanies_ExecutionCompanyID",
                        column: x => x.ExecutionCompanyID,
                        principalTable: "ExecutionCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LogReportResults_LogFiles_LogFileID",
                        column: x => x.LogFileID,
                        principalTable: "LogFiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogReports",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogFileID = table.Column<Guid>(type: "uuid", nullable: false),
                    UserUUID = table.Column<Guid>(type: "uuid", nullable: false),
                    PilotDescription = table.Column<string>(type: "text", nullable: true),
                    ReporterDescription = table.Column<string>(type: "text", nullable: true),
                    ImageUrls = table.Column<string[]>(type: "text[]", nullable: true),
                    AccidentTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogReports", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LogReports_LogFiles_LogFileID",
                        column: x => x.LogFileID,
                        principalTable: "LogFiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Part",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Group = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Part", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LogResultDetail",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartErrorID = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Detail = table.Column<string>(type: "text", nullable: true),
                    Resolve = table.Column<string>(type: "text", nullable: true),
                    LogReportResultID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogResultDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LogResultDetail_LogReportResults_LogReportResultID",
                        column: x => x.LogReportResultID,
                        principalTable: "LogReportResults",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LogResultDetail_Part_PartErrorID",
                        column: x => x.PartErrorID,
                        principalTable: "Part",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogDetails_LogFileID",
                table: "LogDetails",
                column: "LogFileID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogReportResults_ExecutionCompanyID",
                table: "LogReportResults",
                column: "ExecutionCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_LogReportResults_LogFileID",
                table: "LogReportResults",
                column: "LogFileID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogReports_LogFileID",
                table: "LogReports",
                column: "LogFileID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogResultDetail_LogReportResultID",
                table: "LogResultDetail",
                column: "LogReportResultID");

            migrationBuilder.CreateIndex(
                name: "IX_LogResultDetail_PartErrorID",
                table: "LogResultDetail",
                column: "PartErrorID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogDetails");

            migrationBuilder.DropTable(
                name: "LogReports");

            migrationBuilder.DropTable(
                name: "LogResultDetail");

            migrationBuilder.DropTable(
                name: "LogReportResults");

            migrationBuilder.DropTable(
                name: "Part");

            migrationBuilder.DropColumn(
                name: "DroneStatus",
                table: "LogFiles");

            migrationBuilder.DropColumn(
                name: "Errors",
                table: "LogFiles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "LogFiles");
        }
    }
}
