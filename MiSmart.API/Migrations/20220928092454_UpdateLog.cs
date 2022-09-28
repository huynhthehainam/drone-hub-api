using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class UpdateLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accel",
                table: "LogDetails");

            migrationBuilder.DropColumn(
                name: "Edge",
                table: "LogDetails");

            migrationBuilder.DropColumn(
                name: "Vibe",
                table: "LogDetails");

            migrationBuilder.AddColumn<string>(
                name: "PartnerCompanyName",
                table: "LogReports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PilotName",
                table: "LogReports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AccelX",
                table: "LogDetails",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AccelY",
                table: "LogDetails",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AccelZ",
                table: "LogDetails",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsBingLocation",
                table: "LogDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "LogDetails",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "LogDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "LogDetails",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Pitch",
                table: "LogDetails",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Roll",
                table: "LogDetails",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VibeX",
                table: "LogDetails",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VibeY",
                table: "LogDetails",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VibeZ",
                table: "LogDetails",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartnerCompanyName",
                table: "LogReports");

            migrationBuilder.DropColumn(
                name: "PilotName",
                table: "LogReports");

            migrationBuilder.DropColumn(
                name: "AccelX",
                table: "LogDetails");

            migrationBuilder.DropColumn(
                name: "AccelY",
                table: "LogDetails");

            migrationBuilder.DropColumn(
                name: "AccelZ",
                table: "LogDetails");

            migrationBuilder.DropColumn(
                name: "IsBingLocation",
                table: "LogDetails");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "LogDetails");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "LogDetails");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "LogDetails");

            migrationBuilder.DropColumn(
                name: "Pitch",
                table: "LogDetails");

            migrationBuilder.DropColumn(
                name: "Roll",
                table: "LogDetails");

            migrationBuilder.DropColumn(
                name: "VibeX",
                table: "LogDetails");

            migrationBuilder.DropColumn(
                name: "VibeY",
                table: "LogDetails");

            migrationBuilder.DropColumn(
                name: "VibeZ",
                table: "LogDetails");

            migrationBuilder.AddColumn<JsonDocument>(
                name: "Accel",
                table: "LogDetails",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<JsonDocument>(
                name: "Edge",
                table: "LogDetails",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<JsonDocument>(
                name: "Vibe",
                table: "LogDetails",
                type: "jsonb",
                nullable: true);
        }
    }
}
