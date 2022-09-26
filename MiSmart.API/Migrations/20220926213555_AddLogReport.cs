using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AddLogReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DeviceID",
                table: "Plans",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

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

            migrationBuilder.AddColumn<bool>(
                name: "isAnalyzed",
                table: "LogFiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

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
                    Conclusion = table.Column<string>(type: "text", nullable: true),
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
                name: "LogTokens",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogFileID = table.Column<Guid>(type: "uuid", nullable: false),
                    UserUUID = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogTokens", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LogTokens_LogFiles_LogFileID",
                        column: x => x.LogFileID,
                        principalTable: "LogFiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Group = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LogResultDetails",
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
                    table.PrimaryKey("PK_LogResultDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LogResultDetails_LogReportResults_LogReportResultID",
                        column: x => x.LogReportResultID,
                        principalTable: "LogReportResults",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LogResultDetails_Parts_PartErrorID",
                        column: x => x.PartErrorID,
                        principalTable: "Parts",
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
                name: "IX_LogResultDetails_LogReportResultID",
                table: "LogResultDetails",
                column: "LogReportResultID");

            migrationBuilder.CreateIndex(
                name: "IX_LogResultDetails_PartErrorID",
                table: "LogResultDetails",
                column: "PartErrorID");

            migrationBuilder.CreateIndex(
                name: "IX_LogTokens_LogFileID",
                table: "LogTokens",
                column: "LogFileID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogDetails");

            migrationBuilder.DropTable(
                name: "LogReports");

            migrationBuilder.DropTable(
                name: "LogResultDetails");

            migrationBuilder.DropTable(
                name: "LogTokens");

            migrationBuilder.DropTable(
                name: "LogReportResults");

            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.DropColumn(
                name: "DroneStatus",
                table: "LogFiles");

            migrationBuilder.DropColumn(
                name: "Errors",
                table: "LogFiles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "LogFiles");

            migrationBuilder.DropColumn(
                name: "isAnalyzed",
                table: "LogFiles");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceID",
                table: "Plans",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
