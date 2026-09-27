using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stocked.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToPantryItemIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PantryItems_IngredientId",
                table: "PantryItems");

            migrationBuilder.CreateIndex(
                name: "IX_PantryItems_IngredientId",
                table: "PantryItems",
                column: "IngredientId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PantryItems_IngredientId",
                table: "PantryItems");

            migrationBuilder.CreateIndex(
                name: "IX_PantryItems_IngredientId",
                table: "PantryItems",
                column: "IngredientId");
        }
    }
}
