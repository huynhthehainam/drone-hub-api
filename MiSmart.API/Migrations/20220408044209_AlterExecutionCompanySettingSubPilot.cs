using Microsoft.EntityFrameworkCore.Migrations;

namespace MiSmart.API.Migrations
{
    public partial class AlterExecutionCompanySettingSubPilot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubPitlotCostPerHectare",
                table: "ExecutionCompanySettings",
                newName: "SubPilotCostPerHectare");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubPilotCostPerHectare",
                table: "ExecutionCompanySettings",
                newName: "SubPitlotCostPerHectare");
        }
    }
}
