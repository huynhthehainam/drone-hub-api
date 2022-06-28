using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AddFlightStatRecordTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "FlightStats",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FlightStatReportRecords",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Images = table.Column<List<string>>(type: "text[]", nullable: true),
                    FlightStatID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightStatReportRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FlightStatReportRecords_FlightStats_FlightStatID",
                        column: x => x.FlightStatID,
                        principalTable: "FlightStats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightStatReportRecords_FlightStatID",
                table: "FlightStatReportRecords",
                column: "FlightStatID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightStatReportRecords");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "FlightStats");
        }
    }
}
