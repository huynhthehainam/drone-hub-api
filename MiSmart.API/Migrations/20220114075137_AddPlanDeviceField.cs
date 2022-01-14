using Microsoft.EntityFrameworkCore.Migrations;

namespace MiSmart.API.Migrations
{
    public partial class AddPlanDeviceField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeviceID",
                table: "Plans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Plans_DeviceID",
                table: "Plans",
                column: "DeviceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Plans_Devices_DeviceID",
                table: "Plans",
                column: "DeviceID",
                principalTable: "Devices",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plans_Devices_DeviceID",
                table: "Plans");

            migrationBuilder.DropIndex(
                name: "IX_Plans_DeviceID",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "DeviceID",
                table: "Plans");
        }
    }
}
