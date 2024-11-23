using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataLoggerDatabase.Migrations
{
    /// <inheritdoc />
    public partial class StationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StationStatus",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Record = table.Column<int>(type: "integer", nullable: false),
                    StartUpCode = table.Column<string>(type: "text", nullable: true),
                    StationId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StationStatus_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StationStatus_StationId",
                table: "StationStatus",
                column: "StationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StationStatus");
        }
    }
}
