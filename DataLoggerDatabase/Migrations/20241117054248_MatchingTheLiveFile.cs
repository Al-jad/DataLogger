using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLoggerDatabase.Migrations
{
    /// <inheritdoc />
    public partial class MatchingTheLiveFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "BatteryVoltage",
                table: "PipesData",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Discharge2",
                table: "PipesData",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Pressure2",
                table: "PipesData",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Temperature",
                table: "PipesData",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatteryVoltage",
                table: "PipesData");

            migrationBuilder.DropColumn(
                name: "Discharge2",
                table: "PipesData");

            migrationBuilder.DropColumn(
                name: "Pressure2",
                table: "PipesData");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "PipesData");
        }
    }
}
