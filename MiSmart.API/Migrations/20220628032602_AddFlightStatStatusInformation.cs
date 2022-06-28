using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AddFlightStatStatusInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StatusUpdatedTime",
                table: "FlightStats",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StatusUpdatedUserUUID",
                table: "FlightStats",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusUpdatedTime",
                table: "FlightStats");

            migrationBuilder.DropColumn(
                name: "StatusUpdatedUserUUID",
                table: "FlightStats");
        }
    }
}
