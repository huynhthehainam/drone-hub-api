using Microsoft.EntityFrameworkCore.Migrations;

namespace MiSmart.API.Migrations
{
    public partial class AddDeviceDirection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Direction",
                table: "TelemetryRecords",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "LastAdditionalInformationString",
                table: "Devices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LastDirection",
                table: "Devices",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Direction",
                table: "TelemetryRecords");

            migrationBuilder.DropColumn(
                name: "LastAdditionalInformationString",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "LastDirection",
                table: "Devices");
        }
    }
}
