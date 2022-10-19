using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MiSmart.API.Migrations
{
    public partial class AddDeviceModelParamCentrifugalDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "DeviceModels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DeviceModelParamCentrifugalDetails",
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
                    table.PrimaryKey("PK_DeviceModelParamCentrifugalDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DeviceModelParamCentrifugalDetails_DeviceModelParams_Device~",
                        column: x => x.DeviceModelParamID,
                        principalTable: "DeviceModelParams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceModelParamCentrifugalDetails_DeviceModelParamID",
                table: "DeviceModelParamCentrifugalDetails",
                column: "DeviceModelParamID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceModelParamCentrifugalDetails");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "DeviceModels");
        }
    }
}
