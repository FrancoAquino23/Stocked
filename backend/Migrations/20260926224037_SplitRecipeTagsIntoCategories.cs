using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stocked.Api.Migrations
{
    /// <inheritdoc />
    public partial class SplitRecipeTagsIntoCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tags",
                table: "Recipes",
                newName: "DishTypes");

            migrationBuilder.AddColumn<string[]>(
                name: "Cuisines",
                table: "Recipes",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "Diets",
                table: "Recipes",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cuisines",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Diets",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "DishTypes",
                table: "Recipes",
                newName: "Tags");
        }
    }
}
