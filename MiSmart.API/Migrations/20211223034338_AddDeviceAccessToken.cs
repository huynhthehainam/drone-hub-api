using Microsoft.EntityFrameworkCore.Migrations;

namespace MiSmart.API.Migrations
{
    public partial class AddDeviceAccessToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "Devices",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "Devices");
        }
    }
}
