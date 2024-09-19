using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLogger.Migrations
{
    /// <inheritdoc />
    public partial class SourceAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceAddress",
                table: "Stations",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceAddress",
                table: "Stations");
        }
    }
}
