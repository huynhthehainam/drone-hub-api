using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AddFlightStatBoundary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Polygon>(
                name: "Boundary",
                table: "FlightStats",
                type: "geometry",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Boundary",
                table: "FlightStats");
        }
    }
}
