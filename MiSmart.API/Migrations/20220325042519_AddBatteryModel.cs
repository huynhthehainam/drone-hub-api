using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MiSmart.API.Migrations
{
    public partial class AddBatteryModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BatteryModelID",
                table: "Batteries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BatteryModels",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    FileUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryModels", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Batteries_BatteryModelID",
                table: "Batteries",
                column: "BatteryModelID");

            migrationBuilder.AddForeignKey(
                name: "FK_Batteries_BatteryModels_BatteryModelID",
                table: "Batteries",
                column: "BatteryModelID",
                principalTable: "BatteryModels",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batteries_BatteryModels_BatteryModelID",
                table: "Batteries");

            migrationBuilder.DropTable(
                name: "BatteryModels");

            migrationBuilder.DropIndex(
                name: "IX_Batteries_BatteryModelID",
                table: "Batteries");

            migrationBuilder.DropColumn(
                name: "BatteryModelID",
                table: "Batteries");
        }
    }
}
