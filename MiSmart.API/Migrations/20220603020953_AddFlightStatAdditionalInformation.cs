using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AddFlightStatAdditionalInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalInformationString",
                table: "FlightStats",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GCSVersion",
                table: "FlightStats",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalInformationString",
                table: "FlightStats");

            migrationBuilder.DropColumn(
                name: "GCSVersion",
                table: "FlightStats");
        }
    }
}
