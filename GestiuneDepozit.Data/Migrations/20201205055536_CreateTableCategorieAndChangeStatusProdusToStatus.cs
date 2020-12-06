using Microsoft.EntityFrameworkCore.Migrations;

namespace GestiuneDepozit.Data.Migrations
{
    public partial class CreateTableCategorieAndChangeStatusProdusToStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produse_StatusProdus_StatusId",
                table: "Produse");

            migrationBuilder.DropTable(
                name: "StatusProdus");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Produse",
                newName: "CategorieId");

            migrationBuilder.RenameIndex(
                name: "IX_Produse_StatusId",
                table: "Produse",
                newName: "IX_Produse_CategorieId");

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

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "NumeStatus" },
                values: new object[] { 1, "bune" });

            migrationBuilder.CreateIndex(
                name: "IX_Categorii_StatusId",
                table: "Categorii",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Status_NumeStatus",
                table: "Status",
                column: "NumeStatus",
                unique: true,
                filter: "[NumeStatus] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Produse_Categorii_CategorieId",
                table: "Produse",
                column: "CategorieId",
                principalTable: "Categorii",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produse_Categorii_CategorieId",
                table: "Produse");

            migrationBuilder.DropTable(
                name: "Categorii");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.RenameColumn(
                name: "CategorieId",
                table: "Produse",
                newName: "StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Produse_CategorieId",
                table: "Produse",
                newName: "IX_Produse_StatusId");

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

            migrationBuilder.InsertData(
                table: "StatusProdus",
                columns: new[] { "Id", "Status" },
                values: new object[] { 1, "bune" });

            migrationBuilder.CreateIndex(
                name: "IX_StatusProdus_Status",
                table: "StatusProdus",
                column: "Status",
                unique: true,
                filter: "[Status] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Produse_StatusProdus_StatusId",
                table: "Produse",
                column: "StatusId",
                principalTable: "StatusProdus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
