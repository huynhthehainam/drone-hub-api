using Microsoft.EntityFrameworkCore.Migrations;

namespace MiSmart.API.Migrations
{
    public partial class RemoveUnit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskAreaUnit",
                table: "FlightStats");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Fields");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaskAreaUnit",
                table: "FlightStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "Fields",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
