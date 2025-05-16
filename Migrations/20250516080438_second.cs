using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchuelerCheckIN2025.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "anwesend",
                table: "Schuelerdatenset",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "anwesend",
                table: "Schuelerdatenset");
        }
    }
}
