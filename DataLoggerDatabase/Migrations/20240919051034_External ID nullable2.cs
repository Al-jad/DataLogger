#nullable disable


using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataLoggerDatabase.Migrations
{
    /// <inheritdoc />
    public partial class ExternalIDnullable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PipesData_Stations_StationId",
                table: "PipesData");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalId",
                table: "Stations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "StationId",
                table: "PipesData",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PipesData_Stations_StationId",
                table: "PipesData",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PipesData_Stations_StationId",
                table: "PipesData");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalId",
                table: "Stations",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "StationId",
                table: "PipesData",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_PipesData_Stations_StationId",
                table: "PipesData",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id");
        }
    }
}
