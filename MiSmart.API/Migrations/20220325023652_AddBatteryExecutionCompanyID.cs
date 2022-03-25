using Microsoft.EntityFrameworkCore.Migrations;

namespace MiSmart.API.Migrations
{
    public partial class AddBatteryExecutionCompanyID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExecutionCompanyID",
                table: "Batteries",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Batteries_ExecutionCompanyID",
                table: "Batteries",
                column: "ExecutionCompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Batteries_ExecutionCompanies_ExecutionCompanyID",
                table: "Batteries",
                column: "ExecutionCompanyID",
                principalTable: "ExecutionCompanies",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batteries_ExecutionCompanies_ExecutionCompanyID",
                table: "Batteries");

            migrationBuilder.DropIndex(
                name: "IX_Batteries_ExecutionCompanyID",
                table: "Batteries");

            migrationBuilder.DropColumn(
                name: "ExecutionCompanyID",
                table: "Batteries");
        }
    }
}
