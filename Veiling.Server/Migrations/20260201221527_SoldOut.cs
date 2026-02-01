using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veiling.Server.Migrations
{
    /// <inheritdoc />
    public partial class SoldOut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SoldOut",
                schema: "identity",
                table: "Kavels",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoldOut",
                schema: "identity",
                table: "Kavels");
        }
    }
}
