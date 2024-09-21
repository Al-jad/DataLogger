#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;


namespace DataLoggerDatabase.Migrations
{
    /// <inheritdoc />
    public partial class restoredfilepaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceFile",
                table: "Stations");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Stations",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataFile",
                table: "Stations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Stations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "Stations",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Lng",
                table: "Stations",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Stations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadedDataFile",
                table: "Stations",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "DataFile",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "UploadedDataFile",
                table: "Stations");

            migrationBuilder.AddColumn<string>(
                name: "SourceFile",
                table: "Stations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
