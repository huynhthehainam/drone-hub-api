using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MiSmart.API.Migrations
{
    public partial class AddExecutionCompanyUserFlightStatTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExecutionCompanyUserFlightStats",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    ExecutionCompanyUserID = table.Column<long>(type: "bigint", nullable: false),
                    FlightStatID = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionCompanyUserFlightStats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ExecutionCompanyUserFlightStats_ExecutionCompanyUsers_Execu~",
                        column: x => x.ExecutionCompanyUserID,
                        principalTable: "ExecutionCompanyUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExecutionCompanyUserFlightStats_FlightStats_FlightStatID",
                        column: x => x.FlightStatID,
                        principalTable: "FlightStats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionCompanyUserFlightStats_ExecutionCompanyUserID",
                table: "ExecutionCompanyUserFlightStats",
                column: "ExecutionCompanyUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionCompanyUserFlightStats_FlightStatID",
                table: "ExecutionCompanyUserFlightStats",
                column: "FlightStatID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExecutionCompanyUserFlightStats");
        }
    }
}
