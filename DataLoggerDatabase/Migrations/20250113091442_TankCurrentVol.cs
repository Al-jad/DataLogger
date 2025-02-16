using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLoggerDatabase.Migrations
{
    /// <inheritdoc />
    public partial class TankCurrentVol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "TankCurrentVol",
                table: "PipesData",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TankCurrentVol",
                table: "PipesData");
        }
    }
}
