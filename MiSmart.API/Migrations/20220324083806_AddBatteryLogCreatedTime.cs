using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MiSmart.API.Migrations
{
    public partial class AddBatteryLogCreatedTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "BatteryLogs",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "BatteryGroupLogs",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "BatteryLogs");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "BatteryGroupLogs");
        }
    }
}
