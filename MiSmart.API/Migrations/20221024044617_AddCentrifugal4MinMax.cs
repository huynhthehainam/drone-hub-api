using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AddCentrifugal4MinMax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "YCentrifugal4Max",
                table: "DeviceModelParams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "YCentrifugal4Min",
                table: "DeviceModelParams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YCentrifugal4Max",
                table: "DeviceModelParams");

            migrationBuilder.DropColumn(
                name: "YCentrifugal4Min",
                table: "DeviceModelParams");
        }
    }
}
