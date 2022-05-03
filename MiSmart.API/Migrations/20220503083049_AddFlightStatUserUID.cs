using Microsoft.EntityFrameworkCore.Migrations;

namespace MiSmart.API.Migrations
{
    public partial class AddFlightStatUserUID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TMUserUID",
                table: "FlightStats",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TMUserUID",
                table: "FlightStats");
        }
    }
}
