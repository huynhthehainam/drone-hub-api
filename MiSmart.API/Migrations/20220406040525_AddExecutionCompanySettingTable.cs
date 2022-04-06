using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MiSmart.API.Migrations
{
    public partial class AddExecutionCompanySettingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExecutionCompanySettings",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MainPilotCostPerHectare = table.Column<double>(type: "double precision", nullable: false),
                    SubPitlotCostPerHectare = table.Column<double>(type: "double precision", nullable: false),
                    CostPerHectare = table.Column<double>(type: "double precision", nullable: false),
                    ExecutionCompanyID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionCompanySettings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ExecutionCompanySettings_ExecutionCompanies_ExecutionCompan~",
                        column: x => x.ExecutionCompanyID,
                        principalTable: "ExecutionCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionCompanySettings_ExecutionCompanyID",
                table: "ExecutionCompanySettings",
                column: "ExecutionCompanyID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExecutionCompanySettings");
        }
    }
}
