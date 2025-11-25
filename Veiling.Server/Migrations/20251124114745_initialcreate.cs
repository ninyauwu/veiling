using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veiling.Server.Migrations
{
    /// <inheritdoc />
    public partial class initialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bedrijven",
                columns: table => new
                {
                    Bedrijfscode = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bedrijfsnaam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KVKnummer = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bedrijven", x => x.Bedrijfscode);
                });

            migrationBuilder.CreateTable(
                name: "Locaties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KlokId = table.Column<int>(type: "int", nullable: false),
                    Actief = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locaties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gebruikers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<int>(type: "int", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bedrijfsbeheerder = table.Column<bool>(type: "bit", nullable: false),
                    Geverifieerd = table.Column<bool>(type: "bit", nullable: false),
                    BedrijfId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gebruikers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gebruikers_Bedrijven_BedrijfId",
                        column: x => x.BedrijfId,
                        principalTable: "Bedrijven",
                        principalColumn: "Bedrijfscode",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Leveranciers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndexOfReliabilityOfInformation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BedrijfId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leveranciers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leveranciers_Bedrijven_BedrijfId",
                        column: x => x.BedrijfId,
                        principalTable: "Bedrijven",
                        principalColumn: "Bedrijfscode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Veilingmeesters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AantalVeilingenBeheerd = table.Column<int>(type: "int", nullable: false),
                    GebruikerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veilingmeesters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veilingmeesters_Gebruikers_GebruikerId",
                        column: x => x.GebruikerId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Veilingen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Klokduur = table.Column<float>(type: "real", nullable: false),
                    StartTijd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTijd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeldPerTickCode = table.Column<float>(type: "real", nullable: false),
                    VeilingmeesterId = table.Column<int>(type: "int", nullable: true),
                    LocatieId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veilingen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veilingen_Locaties_LocatieId",
                        column: x => x.LocatieId,
                        principalTable: "Locaties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Veilingen_Veilingmeesters_VeilingmeesterId",
                        column: x => x.VeilingmeesterId,
                        principalTable: "Veilingmeesters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Kavels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Beschrijving = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArtikelKenmerken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GekochteContainers = table.Column<int>(type: "int", nullable: false),
                    MinimumPrijs = table.Column<float>(type: "real", nullable: false),
                    MaximumPrijs = table.Column<float>(type: "real", nullable: false),
                    GekochtPrijs = table.Column<float>(type: "real", nullable: false),
                    Minimumhoeveelheid = table.Column<int>(type: "int", nullable: false),
                    Foto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kavelkleur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Karnummer = table.Column<int>(type: "int", nullable: false),
                    Rijnummer = table.Column<int>(type: "int", nullable: false),
                    HoeveelheidContainers = table.Column<int>(type: "int", nullable: false),
                    AantalProductenPerContainer = table.Column<int>(type: "int", nullable: false),
                    LengteVanBloemen = table.Column<float>(type: "real", nullable: false),
                    GewichtVanBloemen = table.Column<float>(type: "real", nullable: false),
                    StageOfMaturity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgsCode = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    Keurcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fustcode = table.Column<int>(type: "int", nullable: false),
                    GeldPerTickCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: true),
                    Reasoning = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VeilingId = table.Column<int>(type: "int", nullable: true),
                    LeverancierId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kavels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kavels_Leveranciers_LeverancierId",
                        column: x => x.LeverancierId,
                        principalTable: "Leveranciers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Kavels_Veilingen_VeilingId",
                        column: x => x.VeilingId,
                        principalTable: "Veilingen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Boden",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Datumtijd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoeveelheidContainers = table.Column<int>(type: "int", nullable: false),
                    Koopprijs = table.Column<float>(type: "real", nullable: false),
                    Betaald = table.Column<bool>(type: "bit", nullable: false),
                    GebruikerId = table.Column<int>(type: "int", nullable: true),
                    KavelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boden", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Boden_Gebruikers_GebruikerId",
                        column: x => x.GebruikerId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Boden_Kavels_KavelId",
                        column: x => x.KavelId,
                        principalTable: "Kavels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boden_GebruikerId",
                table: "Boden",
                column: "GebruikerId");

            migrationBuilder.CreateIndex(
                name: "IX_Boden_KavelId",
                table: "Boden",
                column: "KavelId");

            migrationBuilder.CreateIndex(
                name: "IX_Gebruikers_BedrijfId",
                table: "Gebruikers",
                column: "BedrijfId");

            migrationBuilder.CreateIndex(
                name: "IX_Kavels_LeverancierId",
                table: "Kavels",
                column: "LeverancierId");

            migrationBuilder.CreateIndex(
                name: "IX_Kavels_VeilingId",
                table: "Kavels",
                column: "VeilingId");

            migrationBuilder.CreateIndex(
                name: "IX_Leveranciers_BedrijfId",
                table: "Leveranciers",
                column: "BedrijfId");

            migrationBuilder.CreateIndex(
                name: "IX_Veilingen_LocatieId",
                table: "Veilingen",
                column: "LocatieId");

            migrationBuilder.CreateIndex(
                name: "IX_Veilingen_VeilingmeesterId",
                table: "Veilingen",
                column: "VeilingmeesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Veilingmeesters_GebruikerId",
                table: "Veilingmeesters",
                column: "GebruikerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Boden");

            migrationBuilder.DropTable(
                name: "Kavels");

            migrationBuilder.DropTable(
                name: "Leveranciers");

            migrationBuilder.DropTable(
                name: "Veilingen");

            migrationBuilder.DropTable(
                name: "Locaties");

            migrationBuilder.DropTable(
                name: "Veilingmeesters");

            migrationBuilder.DropTable(
                name: "Gebruikers");

            migrationBuilder.DropTable(
                name: "Bedrijven");
        }
    }
}
