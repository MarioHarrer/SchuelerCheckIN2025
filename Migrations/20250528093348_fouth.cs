using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchuelerCheckIN2025.Migrations
{
    /// <inheritdoc />
    public partial class fouth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "zeit",
                table: "Schuelerdatenset",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "zeit",
                table: "Schuelerdatenset");
        }
    }
}
