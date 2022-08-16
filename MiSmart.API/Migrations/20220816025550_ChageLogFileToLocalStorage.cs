using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class ChageLogFileToLocalStorage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicinesString",
                table: "FlightStats");

            migrationBuilder.RenameColumn(
                name: "FileUrl",
                table: "LogFiles",
                newName: "FileName");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "LogFiles",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte[]>(
                name: "FileBytes",
                table: "LogFiles",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LoggingTime",
                table: "LogFiles",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "LogFiles");

            migrationBuilder.DropColumn(
                name: "FileBytes",
                table: "LogFiles");

            migrationBuilder.DropColumn(
                name: "LoggingTime",
                table: "LogFiles");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "LogFiles",
                newName: "FileUrl");

            migrationBuilder.AddColumn<string>(
                name: "MedicinesString",
                table: "FlightStats",
                type: "text",
                nullable: true);
        }
    }
}
