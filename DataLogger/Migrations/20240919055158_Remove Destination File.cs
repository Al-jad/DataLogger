using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLogger.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDestinationFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationFile",
                table: "Stations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DestinationFile",
                table: "Stations",
                type: "text",
                nullable: true);
        }
    }
}
