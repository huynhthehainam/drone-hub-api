using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MiSmart.API.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DeviceModels",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    FileUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceModels", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ExecutionCompanies",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionCompanies", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CustomerUsers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    CustomerID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerUsers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomerUsers_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExecutionCompanyUsers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    ExecutionCompanyID = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionCompanyUsers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ExecutionCompanyUsers_ExecutionCompanies_ExecutionCompanyID",
                        column: x => x.ExecutionCompanyID,
                        principalTable: "ExecutionCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FieldName = table.Column<string>(type: "text", nullable: true),
                    FieldLocation = table.Column<string>(type: "text", nullable: true),
                    PilotName = table.Column<string>(type: "text", nullable: true),
                    MappingArea = table.Column<double>(type: "double precision", nullable: false),
                    MappingTime = table.Column<double>(type: "double precision", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CustomerID = table.Column<int>(type: "integer", nullable: false),
                    ExecutionCompanyID = table.Column<int>(type: "integer", nullable: false),
                    Border = table.Column<Polygon>(type: "geography (polygon)", nullable: true),
                    Flyway = table.Column<LineString>(type: "geography (linestring)", nullable: true),
                    GPSPoints = table.Column<MultiPoint>(type: "geography (multipoint)", nullable: true),
                    LocationPoint = table.Column<Point>(type: "geography (point)", nullable: true),
                    CalibrationPoints = table.Column<MultiPoint>(type: "geography (multipoint)", nullable: true),
                    WorkSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WorkArea = table.Column<double>(type: "double precision", nullable: false),
                    InnerArea = table.Column<double>(type: "double precision", nullable: false),
                    SprayWidth = table.Column<double>(type: "double precision", nullable: false),
                    SprayDir = table.Column<double>(type: "double precision", nullable: false),
                    IsLargeFarm = table.Column<bool>(type: "boolean", nullable: false),
                    EdgeOffset = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Fields_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fields_ExecutionCompanies_ExecutionCompanyID",
                        column: x => x.ExecutionCompanyID,
                        principalTable: "ExecutionCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    TotalTaskArea = table.Column<double>(type: "double precision", nullable: false),
                    TotalFlightDuration = table.Column<double>(type: "double precision", nullable: false),
                    TotalFlights = table.Column<long>(type: "bigint", nullable: false),
                    ExecutionCompanyID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Teams_ExecutionCompanies_ExecutionCompanyID",
                        column: x => x.ExecutionCompanyID,
                        principalTable: "ExecutionCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamUsers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeamID = table.Column<long>(type: "bigint", nullable: false),
                    ExecutionCompanyUserID = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamUsers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TeamUsers_ExecutionCompanyUsers_ExecutionCompanyUserID",
                        column: x => x.ExecutionCompanyUserID,
                        principalTable: "ExecutionCompanyUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamUsers_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UUID = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TeamID = table.Column<long>(type: "bigint", nullable: true),
                    CustomerID = table.Column<int>(type: "integer", nullable: false),
                    ExecutionCompanyID = table.Column<int>(type: "integer", nullable: true),
                    AccessToken = table.Column<string>(type: "text", nullable: true),
                    NextGeneratingAccessTokenTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeviceModelID = table.Column<int>(type: "integer", nullable: false),
                    LastGroupID = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Devices_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Devices_DeviceModels_DeviceModelID",
                        column: x => x.DeviceModelID,
                        principalTable: "DeviceModels",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Devices_ExecutionCompanies_ExecutionCompanyID",
                        column: x => x.ExecutionCompanyID,
                        principalTable: "ExecutionCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Devices_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FlightStats",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FlightTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TaskLocation = table.Column<string>(type: "text", nullable: true),
                    Flights = table.Column<int>(type: "integer", nullable: false),
                    FieldName = table.Column<string>(type: "text", nullable: true),
                    DeviceName = table.Column<string>(type: "text", nullable: true),
                    TaskArea = table.Column<double>(type: "double precision", nullable: false),
                    FlightDuration = table.Column<double>(type: "double precision", nullable: false),
                    PilotName = table.Column<string>(type: "text", nullable: true),
                    FlywayPoints = table.Column<LineString>(type: "geography (linestring)", nullable: true),
                    DeviceID = table.Column<int>(type: "integer", nullable: false),
                    CustomerID = table.Column<int>(type: "integer", nullable: false),
                    ExecutionCompanyID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightStats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FlightStats_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightStats_Devices_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightStats_ExecutionCompanies_ExecutionCompanyID",
                        column: x => x.ExecutionCompanyID,
                        principalTable: "ExecutionCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogFiles",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: true),
                    DeviceID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogFiles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LogFiles_Devices_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Location = table.Column<Point>(type: "geography (point)", nullable: true),
                    FileBytes = table.Column<byte[]>(type: "bytea", nullable: true),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    DeviceID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Plans_Devices_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelemetryGroups",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceID = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetryGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TelemetryGroups_Devices_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelemetryRecords",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LocationPoint = table.Column<Point>(type: "geography (point)", nullable: true),
                    Direction = table.Column<double>(type: "double precision", nullable: false),
                    AdditionalInformationString = table.Column<string>(type: "text", nullable: true),
                    GroupID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetryRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TelemetryRecords_TelemetryGroups_GroupID",
                        column: x => x.GroupID,
                        principalTable: "TelemetryGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerUsers_CustomerID",
                table: "CustomerUsers",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerUsers_UserID",
                table: "CustomerUsers",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_CustomerID",
                table: "Devices",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceModelID",
                table: "Devices",
                column: "DeviceModelID");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ExecutionCompanyID",
                table: "Devices",
                column: "ExecutionCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_LastGroupID",
                table: "Devices",
                column: "LastGroupID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_TeamID",
                table: "Devices",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionCompanyUsers_ExecutionCompanyID",
                table: "ExecutionCompanyUsers",
                column: "ExecutionCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionCompanyUsers_UserID",
                table: "ExecutionCompanyUsers",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fields_CustomerID",
                table: "Fields",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_ExecutionCompanyID",
                table: "Fields",
                column: "ExecutionCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_FlightStats_CustomerID",
                table: "FlightStats",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_FlightStats_DeviceID",
                table: "FlightStats",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_FlightStats_ExecutionCompanyID",
                table: "FlightStats",
                column: "ExecutionCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_LogFiles_DeviceID",
                table: "LogFiles",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_DeviceID",
                table: "Plans",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_ExecutionCompanyID",
                table: "Teams",
                column: "ExecutionCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_TeamUsers_ExecutionCompanyUserID",
                table: "TeamUsers",
                column: "ExecutionCompanyUserID");

            migrationBuilder.CreateIndex(
                name: "IX_TeamUsers_TeamID_ExecutionCompanyUserID",
                table: "TeamUsers",
                columns: new[] { "TeamID", "ExecutionCompanyUserID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryGroups_DeviceID",
                table: "TelemetryGroups",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryRecords_GroupID",
                table: "TelemetryRecords",
                column: "GroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_TelemetryGroups_LastGroupID",
                table: "Devices",
                column: "LastGroupID",
                principalTable: "TelemetryGroups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Customers_CustomerID",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceModels_DeviceModelID",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_ExecutionCompanies_ExecutionCompanyID",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_ExecutionCompanies_ExecutionCompanyID",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Teams_TeamID",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_TelemetryGroups_LastGroupID",
                table: "Devices");

            migrationBuilder.DropTable(
                name: "CustomerUsers");

            migrationBuilder.DropTable(
                name: "Fields");

            migrationBuilder.DropTable(
                name: "FlightStats");

            migrationBuilder.DropTable(
                name: "LogFiles");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropTable(
                name: "TeamUsers");

            migrationBuilder.DropTable(
                name: "TelemetryRecords");

            migrationBuilder.DropTable(
                name: "ExecutionCompanyUsers");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "DeviceModels");

            migrationBuilder.DropTable(
                name: "ExecutionCompanies");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "TelemetryGroups");

            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
