using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veiling.Server.Migrations
{
    /// <inheritdoc />
    public partial class LeverancierCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leveranciers_Bedrijven_BedrijfId",
                table: "Leveranciers");

            migrationBuilder.AddForeignKey(
                name: "FK_Leveranciers_Bedrijven_BedrijfId",
                table: "Leveranciers",
                column: "BedrijfId",
                principalTable: "Bedrijven",
                principalColumn: "Bedrijfscode",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leveranciers_Bedrijven_BedrijfId",
                table: "Leveranciers");

            migrationBuilder.AddForeignKey(
                name: "FK_Leveranciers_Bedrijven_BedrijfId",
                table: "Leveranciers",
                column: "BedrijfId",
                principalTable: "Bedrijven",
                principalColumn: "Bedrijfscode",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
