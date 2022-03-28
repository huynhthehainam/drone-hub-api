using Microsoft.EntityFrameworkCore.Migrations;

namespace MiSmart.API.Migrations
{
    public partial class UpdateDeviceLastGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_TelemetryGroups_LastGroupID",
                table: "Devices");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_TelemetryGroups_LastGroupID",
                table: "Devices",
                column: "LastGroupID",
                principalTable: "TelemetryGroups",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_TelemetryGroups_LastGroupID",
                table: "Devices");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_TelemetryGroups_LastGroupID",
                table: "Devices",
                column: "LastGroupID",
                principalTable: "TelemetryGroups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
