using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MiSmart.API.Migrations
{
    public partial class AddNextGeneratingAccessTokenTimeField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NextGeneratingAccessTokenTime",
                table: "Devices",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextGeneratingAccessTokenTime",
                table: "Devices");
        }
    }
}
