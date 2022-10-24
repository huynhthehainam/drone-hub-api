using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AddDeviceModelParamTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceModelParams",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    YMin = table.Column<double>(type: "double precision", nullable: false),
                    YMax = table.Column<double>(type: "double precision", nullable: false),
                    FuelLevelNumber = table.Column<double>(type: "double precision", nullable: false),
                    DeviceModelID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceModelParams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DeviceModelParams_DeviceModels_DeviceModelID",
                        column: x => x.DeviceModelID,
                        principalTable: "DeviceModels",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceModelParamDetails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    XMin = table.Column<double>(type: "double precision", nullable: false),
                    XMax = table.Column<double>(type: "double precision", nullable: false),
                    A = table.Column<double>(type: "double precision", nullable: false),
                    B = table.Column<double>(type: "double precision", nullable: false),
                    DeviceModelParamID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceModelParamDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DeviceModelParamDetails_DeviceModelParams_DeviceModelParamID",
                        column: x => x.DeviceModelParamID,
                        principalTable: "DeviceModelParams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceModelParamDetails_DeviceModelParamID",
                table: "DeviceModelParamDetails",
                column: "DeviceModelParamID");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceModelParams_DeviceModelID",
                table: "DeviceModelParams",
                column: "DeviceModelID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceModelParamDetails");

            migrationBuilder.DropTable(
                name: "DeviceModelParams");
        }
    }
}
