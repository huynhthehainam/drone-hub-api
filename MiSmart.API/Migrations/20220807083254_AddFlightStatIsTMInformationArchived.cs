using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AddFlightStatIsTMInformationArchived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTMInformationArchived",
                table: "FlightStats",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTMInformationArchived",
                table: "FlightStats");
        }
    }
}
