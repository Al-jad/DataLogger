using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLoggerDatabase.Migrations
{
    /// <inheritdoc />
    public partial class WaterLevelAndQuality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "WaterLevel",
                table: "PipesData",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "WaterQuality",
                table: "PipesData",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaterLevel",
                table: "PipesData");

            migrationBuilder.DropColumn(
                name: "WaterQuality",
                table: "PipesData");
        }
    }
}
