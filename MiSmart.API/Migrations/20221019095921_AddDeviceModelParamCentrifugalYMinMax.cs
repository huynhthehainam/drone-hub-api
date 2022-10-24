using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AddDeviceModelParamCentrifugalYMinMax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "YCentrifugalMax",
                table: "DeviceModelParams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "YCentrifugalMin",
                table: "DeviceModelParams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YCentrifugalMax",
                table: "DeviceModelParams");

            migrationBuilder.DropColumn(
                name: "YCentrifugalMin",
                table: "DeviceModelParams");
        }
    }
}
