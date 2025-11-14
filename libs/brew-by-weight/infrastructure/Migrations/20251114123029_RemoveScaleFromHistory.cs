using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicraPro.BrewByWeight.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveScaleFromHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ScaleId", table: "ProcessEntries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScaleId",
                table: "ProcessEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );
        }
    }
}
