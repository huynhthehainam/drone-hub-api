using Microsoft.EntityFrameworkCore.Migrations;

namespace MiSmart.API.Migrations
{
    public partial class AddFlightStatTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightStats_ExecutionCompanies_ExecutionCompanyID",
                table: "FlightStats");

            migrationBuilder.AddColumn<long>(
                name: "TeamID",
                table: "FlightStats",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlightStats_TeamID",
                table: "FlightStats",
                column: "TeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightStats_ExecutionCompanies_ExecutionCompanyID",
                table: "FlightStats",
                column: "ExecutionCompanyID",
                principalTable: "ExecutionCompanies",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FlightStats_Teams_TeamID",
                table: "FlightStats",
                column: "TeamID",
                principalTable: "Teams",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightStats_ExecutionCompanies_ExecutionCompanyID",
                table: "FlightStats");

            migrationBuilder.DropForeignKey(
                name: "FK_FlightStats_Teams_TeamID",
                table: "FlightStats");

            migrationBuilder.DropIndex(
                name: "IX_FlightStats_TeamID",
                table: "FlightStats");

            migrationBuilder.DropColumn(
                name: "TeamID",
                table: "FlightStats");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightStats_ExecutionCompanies_ExecutionCompanyID",
                table: "FlightStats",
                column: "ExecutionCompanyID",
                principalTable: "ExecutionCompanies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
