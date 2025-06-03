using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicraPro.BeanManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoasteryEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoasteryEntries", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "BeanEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    RoasteryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CountryCode = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeanEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeanEntries_RoasteryEntries_RoasteryId",
                        column: x => x.RoasteryId,
                        principalTable: "RoasteryEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "RecipeEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BeanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Discriminator = table.Column<string>(
                        type: "TEXT",
                        maxLength: 21,
                        nullable: false
                    ),
                    GrindSetting = table.Column<double>(type: "REAL", nullable: true),
                    CoffeeQuantity = table.Column<double>(type: "REAL", nullable: true),
                    InCupQuantity = table.Column<double>(type: "REAL", nullable: true),
                    BrewTemperature = table.Column<double>(type: "REAL", nullable: true),
                    TargetExtractionTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    V60RecipeDb_GrindSetting = table.Column<double>(type: "REAL", nullable: true),
                    V60RecipeDb_CoffeeQuantity = table.Column<double>(type: "REAL", nullable: true),
                    V60RecipeDb_InCupQuantity = table.Column<double>(type: "REAL", nullable: true),
                    V60RecipeDb_BrewTemperature = table.Column<double>(
                        type: "REAL",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeEntries_BeanEntries_BeanId",
                        column: x => x.BeanId,
                        principalTable: "BeanEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_BeanEntries_RoasteryId",
                table: "BeanEntries",
                column: "RoasteryId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_RecipeEntries_BeanId",
                table: "RecipeEntries",
                column: "BeanId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "RecipeEntries");

            migrationBuilder.DropTable(name: "BeanEntries");

            migrationBuilder.DropTable(name: "RoasteryEntries");
        }
    }
}
