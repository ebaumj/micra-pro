﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicraPro.BeanManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BeanManagementKeyValueStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KeyValueEntries",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    JsonValue = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyValueEntries", x => x.Key);
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "KeyValueEntries");
        }
    }
}
