using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicraPro.Shared.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigurationEntries",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    JsonValue = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationEntries", x => x.Key);
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ConfigurationEntries");
        }
    }
}
