using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AddFlightStatIsBoundaryArchived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBoundaryArchived",
                table: "FlightStats",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBoundaryArchived",
                table: "FlightStats");
        }
    }
}
