using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stocked.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddReadyInMinutesToRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReadyInMinutes",
                table: "Recipes",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReadyInMinutes",
                table: "Recipes");
        }
    }
}
