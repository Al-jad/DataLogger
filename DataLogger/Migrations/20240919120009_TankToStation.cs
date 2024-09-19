using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataLogger.Migrations
{
    /// <inheritdoc />
    public partial class TankToStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TankData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Record = table.Column<int>(type: "integer", nullable: false),
                    StationId = table.Column<long>(type: "bigint", nullable: false),
                    WL = table.Column<float>(type: "real", nullable: true),
                    TotalVolumePerHour = table.Column<float>(type: "real", nullable: true),
                    TotalVolumePerDay = table.Column<float>(type: "real", nullable: true),
                    Turbidity = table.Column<float>(type: "real", nullable: true),
                    ElectricConductivity = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TankData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TankData_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TankData_StationId",
                table: "TankData",
                column: "StationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TankData");
        }
    }
}
