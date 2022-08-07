using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AlterFlightStatTMUserUUID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TMUserUID",
                table: "FlightStats",
                newName: "TMUserUUID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TMUserUUID",
                table: "FlightStats",
                newName: "TMUserUID");
        }
    }
}
