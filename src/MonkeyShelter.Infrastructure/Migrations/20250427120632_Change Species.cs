using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonkeyShelter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSpecies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Monkeys_Species_SpeciesId",
                table: "Monkeys");

            migrationBuilder.RenameColumn(
                name: "SpeciesId",
                table: "Monkeys",
                newName: "SpecieId");

            migrationBuilder.RenameIndex(
                name: "IX_Monkeys_SpeciesId",
                table: "Monkeys",
                newName: "IX_Monkeys_SpecieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Monkeys_Species_SpecieId",
                table: "Monkeys",
                column: "SpecieId",
                principalTable: "Species",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Monkeys_Species_SpecieId",
                table: "Monkeys");

            migrationBuilder.RenameColumn(
                name: "SpecieId",
                table: "Monkeys",
                newName: "SpeciesId");

            migrationBuilder.RenameIndex(
                name: "IX_Monkeys_SpecieId",
                table: "Monkeys",
                newName: "IX_Monkeys_SpeciesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Monkeys_Species_SpeciesId",
                table: "Monkeys",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
