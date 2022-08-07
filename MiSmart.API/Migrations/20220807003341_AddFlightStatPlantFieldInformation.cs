using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AddFlightStatPlantFieldInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<JsonDocument>(
                name: "TMField",
                table: "FlightStats",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TMFieldID",
                table: "FlightStats",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<JsonDocument>(
                name: "TMPlant",
                table: "FlightStats",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TMPlantID",
                table: "FlightStats",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TMField",
                table: "FlightStats");

            migrationBuilder.DropColumn(
                name: "TMFieldID",
                table: "FlightStats");

            migrationBuilder.DropColumn(
                name: "TMPlant",
                table: "FlightStats");

            migrationBuilder.DropColumn(
                name: "TMPlantID",
                table: "FlightStats");
        }
    }
}
