using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Charts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HumidityChartsData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HumidityChartsData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IlluminationChartsData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IlluminationChartsData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoilHumidityChartsData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoilHumidityChartsData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemperatureChartsData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemperatureChartsData", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HumidityChartsData_DateTime",
                table: "HumidityChartsData",
                column: "DateTime");

            migrationBuilder.CreateIndex(
                name: "IX_IlluminationChartsData_DateTime",
                table: "IlluminationChartsData",
                column: "DateTime");

            migrationBuilder.CreateIndex(
                name: "IX_SoilHumidityChartsData_DateTime",
                table: "SoilHumidityChartsData",
                column: "DateTime");

            migrationBuilder.CreateIndex(
                name: "IX_TemperatureChartsData_DateTime",
                table: "TemperatureChartsData",
                column: "DateTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HumidityChartsData");

            migrationBuilder.DropTable(
                name: "IlluminationChartsData");

            migrationBuilder.DropTable(
                name: "SoilHumidityChartsData");

            migrationBuilder.DropTable(
                name: "TemperatureChartsData");
        }
    }
}
