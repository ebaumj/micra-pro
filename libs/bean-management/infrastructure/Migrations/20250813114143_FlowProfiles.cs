using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicraPro.BeanManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FlowProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlowProfileEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RecipeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StartFlow = table.Column<double>(type: "REAL", nullable: false),
                    FlowSettings = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowProfileEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowProfileEntries_RecipeEntries_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "RecipeEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_FlowProfileEntries_RecipeId",
                table: "FlowProfileEntries",
                column: "RecipeId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "FlowProfileEntries");
        }
    }
}
