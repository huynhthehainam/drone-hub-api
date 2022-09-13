using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AddFlightStatBatteryInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BatteryID",
                table: "FlightStats",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CycleCount",
                table: "FlightStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FlightStats_BatteryID",
                table: "FlightStats",
                column: "BatteryID");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightStats_Batteries_BatteryID",
                table: "FlightStats",
                column: "BatteryID",
                principalTable: "Batteries",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightStats_Batteries_BatteryID",
                table: "FlightStats");

            migrationBuilder.DropIndex(
                name: "IX_FlightStats_BatteryID",
                table: "FlightStats");

            migrationBuilder.DropColumn(
                name: "BatteryID",
                table: "FlightStats");

            migrationBuilder.DropColumn(
                name: "CycleCount",
                table: "FlightStats");
        }
    }
}
