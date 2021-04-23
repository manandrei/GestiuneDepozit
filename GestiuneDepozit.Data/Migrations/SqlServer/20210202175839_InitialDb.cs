using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GestiuneDepozit.Data.Migrations.SqlServer
{
    public partial class InitialDb : Migration
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
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeStatus = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categorii",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeCategorie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorii", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categorii_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    CategorieId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produse_Categorii_CategorieId",
                        column: x => x.CategorieId,
                        principalTable: "Categorii",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Produse_Locatii_LocatieId",
                        column: x => x.LocatieId,
                        principalTable: "Locatii",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "NumeStatus" },
                values: new object[] { 1, "bune" });

            migrationBuilder.CreateIndex(
                name: "IX_Categorii_StatusId",
                table: "Categorii",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Locatii_NumeLocatie",
                table: "Locatii",
                column: "NumeLocatie",
                unique: true,
                filter: "[NumeLocatie] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Produse_CategorieId",
                table: "Produse",
                column: "CategorieId");

            migrationBuilder.CreateIndex(
                name: "IX_Produse_LocatieId",
                table: "Produse",
                column: "LocatieId");

            migrationBuilder.CreateIndex(
                name: "IX_Status_NumeStatus",
                table: "Status",
                column: "NumeStatus",
                unique: true,
                filter: "[NumeStatus] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Produse");

            migrationBuilder.DropTable(
                name: "Categorii");

            migrationBuilder.DropTable(
                name: "Locatii");

            migrationBuilder.DropTable(
                name: "Status");
        }
    }
}
