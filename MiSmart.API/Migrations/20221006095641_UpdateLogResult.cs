using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class UpdateLogResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnalystName",
                table: "LogReportResults",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApproverName",
                table: "LogReportResults",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "LogReportResults",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnalystName",
                table: "LogReportResults");

            migrationBuilder.DropColumn(
                name: "ApproverName",
                table: "LogReportResults");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "LogReportResults");
        }
    }
}
