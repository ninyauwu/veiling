using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veiling.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bedrijven",
                schema: "identity",
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
                schema: "identity",
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
                name: "AspNetRoleClaims",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bedrijfsbeheerder = table.Column<bool>(type: "bit", nullable: false),
                    Geverifieerd = table.Column<bool>(type: "bit", nullable: false),
                    BedrijfId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Bedrijven_BedrijfId",
                        column: x => x.BedrijfId,
                        principalSchema: "identity",
                        principalTable: "Bedrijven",
                        principalColumn: "Bedrijfscode",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Leveranciers",
                schema: "identity",
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
                        principalSchema: "identity",
                        principalTable: "Bedrijven",
                        principalColumn: "Bedrijfscode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "identity",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Veilingmeesters",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AantalVeilingenBeheerd = table.Column<int>(type: "int", nullable: false),
                    GebruikerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veilingmeesters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veilingmeesters_AspNetUsers_GebruikerId",
                        column: x => x.GebruikerId,
                        principalSchema: "identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Veilingen",
                schema: "identity",
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
                        principalSchema: "identity",
                        principalTable: "Locaties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Veilingen_Veilingmeesters_VeilingmeesterId",
                        column: x => x.VeilingmeesterId,
                        principalSchema: "identity",
                        principalTable: "Veilingmeesters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Kavels",
                schema: "identity",
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
                    LocatieId = table.Column<int>(type: "int", nullable: false),
                    VeilingId = table.Column<int>(type: "int", nullable: true),
                    LeverancierId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kavels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kavels_Leveranciers_LeverancierId",
                        column: x => x.LeverancierId,
                        principalSchema: "identity",
                        principalTable: "Leveranciers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Kavels_Veilingen_VeilingId",
                        column: x => x.VeilingId,
                        principalSchema: "identity",
                        principalTable: "Veilingen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Boden",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Datumtijd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoeveelheidContainers = table.Column<int>(type: "int", nullable: false),
                    Koopprijs = table.Column<float>(type: "real", nullable: false),
                    Betaald = table.Column<bool>(type: "bit", nullable: false),
                    GebruikerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    KavelVeilingId = table.Column<int>(type: "int", nullable: true),
                    AankoopId = table.Column<int>(type: "int", nullable: true),
                    KavelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boden", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Boden_AspNetUsers_GebruikerId",
                        column: x => x.GebruikerId,
                        principalSchema: "identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Boden_Kavels_KavelId",
                        column: x => x.KavelId,
                        principalSchema: "identity",
                        principalTable: "Kavels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Aankopen",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Hoeveelheid = table.Column<int>(type: "int", nullable: false),
                    BodId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aankopen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aankopen_Boden_BodId",
                        column: x => x.BodId,
                        principalSchema: "identity",
                        principalTable: "Boden",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "KavelVeilingen",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMs = table.Column<int>(type: "int", nullable: false),
                    KavelId = table.Column<int>(type: "int", nullable: false),
                    BodId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KavelVeilingen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KavelVeilingen_Boden_BodId",
                        column: x => x.BodId,
                        principalSchema: "identity",
                        principalTable: "Boden",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KavelVeilingen_Kavels_KavelId",
                        column: x => x.KavelId,
                        principalSchema: "identity",
                        principalTable: "Kavels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aankopen_BodId",
                schema: "identity",
                table: "Aankopen",
                column: "BodId",
                unique: true,
                filter: "[BodId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "identity",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "identity",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "identity",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "identity",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "identity",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "identity",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BedrijfId",
                schema: "identity",
                table: "AspNetUsers",
                column: "BedrijfId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "identity",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Boden_GebruikerId",
                schema: "identity",
                table: "Boden",
                column: "GebruikerId");

            migrationBuilder.CreateIndex(
                name: "IX_Boden_KavelId",
                schema: "identity",
                table: "Boden",
                column: "KavelId");

            migrationBuilder.CreateIndex(
                name: "IX_Kavels_LeverancierId",
                schema: "identity",
                table: "Kavels",
                column: "LeverancierId");

            migrationBuilder.CreateIndex(
                name: "IX_Kavels_VeilingId",
                schema: "identity",
                table: "Kavels",
                column: "VeilingId");

            migrationBuilder.CreateIndex(
                name: "IX_KavelVeilingen_BodId",
                schema: "identity",
                table: "KavelVeilingen",
                column: "BodId",
                unique: true,
                filter: "[BodId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_KavelVeilingen_KavelId",
                schema: "identity",
                table: "KavelVeilingen",
                column: "KavelId");

            migrationBuilder.CreateIndex(
                name: "IX_Leveranciers_BedrijfId",
                schema: "identity",
                table: "Leveranciers",
                column: "BedrijfId");

            migrationBuilder.CreateIndex(
                name: "IX_Veilingen_LocatieId",
                schema: "identity",
                table: "Veilingen",
                column: "LocatieId");

            migrationBuilder.CreateIndex(
                name: "IX_Veilingen_VeilingmeesterId",
                schema: "identity",
                table: "Veilingen",
                column: "VeilingmeesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Veilingmeesters_GebruikerId",
                schema: "identity",
                table: "Veilingmeesters",
                column: "GebruikerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aankopen",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "KavelVeilingen",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Boden",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Kavels",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Leveranciers",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Veilingen",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Locaties",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Veilingmeesters",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Bedrijven",
                schema: "identity");
        }
    }
}
