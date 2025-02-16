using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLoggerDatabase.Migrations
{
    /// <inheritdoc />
    public partial class StationTypeRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StationType",
                table: "Stations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StationType",
                table: "Stations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
