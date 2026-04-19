using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace SampleTracker.API.Data.Migrations
{
    [Migration("20240101000000_InitialCreate")]
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Methods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Instrument = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Methods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Samples",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SampleId = table.Column<string>(type: "TEXT", nullable: false),
                    Matrix = table.Column<string>(type: "TEXT", nullable: false),
                    CollectedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    MethodId = table.Column<int>(type: "INTEGER", nullable: true),
                    AnalystId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Samples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Samples_Methods_MethodId",
                        column: x => x.MethodId,
                        principalTable: "Methods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Samples_Users_AnalystId",
                        column: x => x.AnalystId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Methods",
                columns: new[] { "Id", "Code", "Instrument", "Name" },
                values: new object[,]
                {
                    { 1, "EPA 200.8", "ICP-MS",        "Metals by ICP-MS" },
                    { 2, "EPA 524.2", "GC-MS",         "VOCs by GC-MS"    },
                    { 3, "EPA 8270D", "GC-MS",         "SVOCs by GC-MS"   },
                    { 4, "EPA 300.0", "IC",             "Anions by IC"     },
                    { 5, "SM 5310B",  "TOC Analyzer",  "TOC"              }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Samples_AnalystId",
                table: "Samples",
                column: "AnalystId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_MethodId",
                table: "Samples",
                column: "MethodId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Samples");
            migrationBuilder.DropTable(name: "Methods");
            migrationBuilder.DropTable(name: "Users");
        }
    }
}
