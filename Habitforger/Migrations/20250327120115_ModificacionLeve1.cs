using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitforger.Migrations
{
    /// <inheritdoc />
    public partial class ModificacionLeve1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaRespuestaFecha",
                table: "Habitos",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UltimaRespuestaFecha",
                table: "Habitos");
        }
    }
}
