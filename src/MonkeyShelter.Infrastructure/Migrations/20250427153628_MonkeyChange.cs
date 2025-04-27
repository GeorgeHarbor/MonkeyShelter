using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonkeyShelter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MonkeyChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DepartureDate",
                table: "Monkeys",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Monkeys",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartureDate",
                table: "Monkeys");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Monkeys");
        }
    }
}
