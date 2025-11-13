using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veiling.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddAfbeeldingUrlToKavel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AantalProductenPerContainer",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "ArtikelKenmerken",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "GekochtPrijs",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "GekochteContainers",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "GeldPerTickCode",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "GewichtVanBloemen",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "HoeveelheidContainers",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "Karnummer",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "Kavelkleur",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "Keurcode",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "LengteVanBloemen",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "MaximumPrijs",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "MinimumPrijs",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "Minimumhoeveelheid",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "NgsCode",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "StageOfMaturity",
                table: "Kavels");

            migrationBuilder.RenameColumn(
                name: "Rijnummer",
                table: "Kavels",
                newName: "Aantal");

            migrationBuilder.AlterColumn<string>(
                name: "Fustcode",
                table: "Kavels",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Beschrijving",
                table: "Kavels",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AfbeeldingUrl",
                table: "Kavels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Kleur",
                table: "Kavels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Kwaliteit",
                table: "Kavels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lengte",
                table: "Kavels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaatsVanVerkoop",
                table: "Kavels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Stadium",
                table: "Kavels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "StartPrijs",
                table: "Kavels",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AfbeeldingUrl",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "Kleur",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "Kwaliteit",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "Lengte",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "PlaatsVanVerkoop",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "Stadium",
                table: "Kavels");

            migrationBuilder.DropColumn(
                name: "StartPrijs",
                table: "Kavels");

            migrationBuilder.RenameColumn(
                name: "Aantal",
                table: "Kavels",
                newName: "Rijnummer");

            migrationBuilder.AlterColumn<int>(
                name: "Fustcode",
                table: "Kavels",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Beschrijving",
                table: "Kavels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AantalProductenPerContainer",
                table: "Kavels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ArtikelKenmerken",
                table: "Kavels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Foto",
                table: "Kavels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "GekochtPrijs",
                table: "Kavels",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "GekochteContainers",
                table: "Kavels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GeldPerTickCode",
                table: "Kavels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "GewichtVanBloemen",
                table: "Kavels",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "HoeveelheidContainers",
                table: "Kavels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Karnummer",
                table: "Kavels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Kavelkleur",
                table: "Kavels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Keurcode",
                table: "Kavels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "LengteVanBloemen",
                table: "Kavels",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MaximumPrijs",
                table: "Kavels",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MinimumPrijs",
                table: "Kavels",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "Minimumhoeveelheid",
                table: "Kavels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NgsCode",
                table: "Kavels",
                type: "nvarchar(1)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StageOfMaturity",
                table: "Kavels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
