using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EspClientSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MinTemperature = table.Column<double>(type: "REAL", nullable: false),
                    MaxTemperature = table.Column<double>(type: "REAL", nullable: false),
                    MinHumidity = table.Column<double>(type: "REAL", nullable: false),
                    MaxHumidity = table.Column<double>(type: "REAL", nullable: false),
                    MinIllumination = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxIllumination = table.Column<int>(type: "INTEGER", nullable: false),
                    MinSoilHumidity = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxSoilHumidity = table.Column<int>(type: "INTEGER", nullable: false),
                    RgbMode = table.Column<int>(type: "INTEGER", nullable: false),
                    RgbColor = table.Column<string>(type: "TEXT", nullable: false),
                    RgbPower = table.Column<bool>(type: "INTEGER", nullable: false),
                    RgbBrightness = table.Column<int>(type: "INTEGER", nullable: false),
                    WindowOpen = table.Column<bool>(type: "INTEGER", nullable: false),
                    FanOn = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspClientSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Key);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EspClientSettings");

            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
