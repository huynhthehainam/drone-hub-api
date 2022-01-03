using Microsoft.EntityFrameworkCore.Migrations;

namespace MiSmart.API.Migrations
{
    public partial class UpdateUserIDConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerUsers_CustomerID_UserID",
                table: "CustomerUsers");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerUsers_CustomerID",
                table: "CustomerUsers",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerUsers_UserID",
                table: "CustomerUsers",
                column: "UserID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerUsers_CustomerID",
                table: "CustomerUsers");

            migrationBuilder.DropIndex(
                name: "IX_CustomerUsers_UserID",
                table: "CustomerUsers");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerUsers_CustomerID_UserID",
                table: "CustomerUsers",
                columns: new[] { "CustomerID", "UserID" },
                unique: true);
        }
    }
}
