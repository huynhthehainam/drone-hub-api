using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MiSmart.API.Migrations
{
    public partial class AddBatteryLastGroupID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LastGroupID",
                table: "Batteries",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Batteries_LastGroupID",
                table: "Batteries",
                column: "LastGroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_Batteries_BatteryGroupLogs_LastGroupID",
                table: "Batteries",
                column: "LastGroupID",
                principalTable: "BatteryGroupLogs",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batteries_BatteryGroupLogs_LastGroupID",
                table: "Batteries");

            migrationBuilder.DropIndex(
                name: "IX_Batteries_LastGroupID",
                table: "Batteries");

            migrationBuilder.DropColumn(
                name: "LastGroupID",
                table: "Batteries");
        }
    }
}
