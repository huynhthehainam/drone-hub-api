using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class FixColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isAnalyzed",
                table: "LogFiles",
                newName: "IsAnalyzed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAnalyzed",
                table: "LogFiles",
                newName: "isAnalyzed");
        }
    }
}
