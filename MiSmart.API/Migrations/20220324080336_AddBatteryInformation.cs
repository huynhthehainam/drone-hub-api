using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MiSmart.API.Migrations
{
    public partial class AddBatteryInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastBatterGroupLogsString",
                table: "Devices",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Batteries",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActualID = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batteries", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BatteryGroupLogs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    BatteryID = table.Column<int>(type: "integer", nullable: false)
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
                    CurrentUnit = table.Column<string>(type: "text", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_BatteryGroupLogs_BatteryID",
                table: "BatteryGroupLogs",
                column: "BatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryLogs_GroupLogID",
                table: "BatteryLogs",
                column: "GroupLogID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatteryLogs");

            migrationBuilder.DropTable(
                name: "BatteryGroupLogs");

            migrationBuilder.DropTable(
                name: "Batteries");

            migrationBuilder.DropColumn(
                name: "LastBatterGroupLogsString",
                table: "Devices");
        }
    }
}
