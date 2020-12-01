using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GestiuneDepozit.Data.Migrations
{
    public partial class InitialDatabaseSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locatii",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeLocatie = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CapacitateProduse = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locatii", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusProdus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusProdus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataInregistrare = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InregistratDe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodProdus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Serie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Saptamana = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    An = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocatieId = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produse_Locatii_LocatieId",
                        column: x => x.LocatieId,
                        principalTable: "Locatii",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Produse_StatusProdus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "StatusProdus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "StatusProdus",
                columns: new[] { "Id", "Status" },
                values: new object[] { 1, "bune" });

            migrationBuilder.CreateIndex(
                name: "IX_Locatii_NumeLocatie",
                table: "Locatii",
                column: "NumeLocatie",
                unique: true,
                filter: "[NumeLocatie] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Produse_LocatieId",
                table: "Produse",
                column: "LocatieId");

            migrationBuilder.CreateIndex(
                name: "IX_Produse_StatusId",
                table: "Produse",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusProdus_Status",
                table: "StatusProdus",
                column: "Status",
                unique: true,
                filter: "[Status] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Produse");

            migrationBuilder.DropTable(
                name: "Locatii");

            migrationBuilder.DropTable(
                name: "StatusProdus");
        }
    }
}
