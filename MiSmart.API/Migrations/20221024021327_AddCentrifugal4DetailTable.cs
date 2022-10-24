using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AddCentrifugal4DetailTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FlowRateMaxLimit",
                table: "DeviceModelParams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FlowRateMiddleLimit",
                table: "DeviceModelParams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FlowRateMinLimit",
                table: "DeviceModelParams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "DeviceModelParamCentrifugal4Details",
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
                    table.PrimaryKey("PK_DeviceModelParamCentrifugal4Details", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DeviceModelParamCentrifugal4Details_DeviceModelParams_Devic~",
                        column: x => x.DeviceModelParamID,
                        principalTable: "DeviceModelParams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceModelParamCentrifugal4Details_DeviceModelParamID",
                table: "DeviceModelParamCentrifugal4Details",
                column: "DeviceModelParamID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceModelParamCentrifugal4Details");

            migrationBuilder.DropColumn(
                name: "FlowRateMaxLimit",
                table: "DeviceModelParams");

            migrationBuilder.DropColumn(
                name: "FlowRateMiddleLimit",
                table: "DeviceModelParams");

            migrationBuilder.DropColumn(
                name: "FlowRateMinLimit",
                table: "DeviceModelParams");
        }
    }
}
