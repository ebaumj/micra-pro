using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicraPro.BrewByWeight.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BeanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScaleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InCupQuantity = table.Column<double>(type: "REAL", nullable: false),
                    GrindSetting = table.Column<double>(type: "REAL", nullable: false),
                    CoffeeQuantity = table.Column<double>(type: "REAL", nullable: false),
                    TargetExtractionTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Spout = table.Column<int>(type: "INTEGER", nullable: false),
                    Discriminator = table.Column<string>(
                        type: "TEXT",
                        maxLength: 21,
                        nullable: false
                    ),
                    CancelledProcessDb_AverageFlow = table.Column<double>(
                        type: "REAL",
                        nullable: true
                    ),
                    CancelledProcessDb_TotalQuantity = table.Column<double>(
                        type: "REAL",
                        nullable: true
                    ),
                    CancelledProcessDb_TotalTime = table.Column<TimeSpan>(
                        type: "TEXT",
                        nullable: true
                    ),
                    FailedProcessDb_AverageFlow = table.Column<double>(
                        type: "REAL",
                        nullable: true
                    ),
                    FailedProcessDb_TotalQuantity = table.Column<double>(
                        type: "REAL",
                        nullable: true
                    ),
                    TotalTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    ErrorType = table.Column<string>(type: "TEXT", nullable: true),
                    AverageFlow = table.Column<double>(type: "REAL", nullable: true),
                    TotalQuantity = table.Column<double>(type: "REAL", nullable: true),
                    ExtractionTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessEntries", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "ProcessRuntimeDataEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProcessId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Flow = table.Column<double>(type: "REAL", nullable: false),
                    TotalQuantity = table.Column<double>(type: "REAL", nullable: false),
                    TotalTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessRuntimeDataEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessRuntimeDataEntries_ProcessEntries_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "ProcessEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProcessRuntimeDataEntries_ProcessId",
                table: "ProcessRuntimeDataEntries",
                column: "ProcessId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ProcessRuntimeDataEntries");

            migrationBuilder.DropTable(name: "ProcessEntries");
        }
    }
}
