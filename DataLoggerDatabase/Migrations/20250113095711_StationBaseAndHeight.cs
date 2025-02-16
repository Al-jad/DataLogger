using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLoggerDatabase.Migrations
{
    /// <inheritdoc />
    public partial class StationBaseAndHeight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "BaseArea",
                table: "Stations",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "TankHeight",
                table: "Stations",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TankSensorReading",
                table: "PipesData",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseArea",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "TankHeight",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "TankSensorReading",
                table: "PipesData");
        }
    }
}
