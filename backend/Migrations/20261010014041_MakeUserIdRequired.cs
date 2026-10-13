using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stocked.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserIdRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteRecipes_Users_UserId",
                table: "FavoriteRecipes");

            migrationBuilder.DropForeignKey(
                name: "FK_MealPlanEntries_Users_UserId",
                table: "MealPlanEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_PantryAdjustments_Users_UserId",
                table: "PantryAdjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_PantryItems_Users_UserId",
                table: "PantryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipePreparations_Users_UserId",
                table: "RecipePreparations");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "RecipePreparations",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "PantryItems",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "PantryAdjustments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "MealPlanEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "FavoriteRecipes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteRecipes_Users_UserId",
                table: "FavoriteRecipes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MealPlanEntries_Users_UserId",
                table: "MealPlanEntries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PantryAdjustments_Users_UserId",
                table: "PantryAdjustments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PantryItems_Users_UserId",
                table: "PantryItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipePreparations_Users_UserId",
                table: "RecipePreparations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteRecipes_Users_UserId",
                table: "FavoriteRecipes");

            migrationBuilder.DropForeignKey(
                name: "FK_MealPlanEntries_Users_UserId",
                table: "MealPlanEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_PantryAdjustments_Users_UserId",
                table: "PantryAdjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_PantryItems_Users_UserId",
                table: "PantryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipePreparations_Users_UserId",
                table: "RecipePreparations");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "RecipePreparations",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "PantryItems",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "PantryAdjustments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "MealPlanEntries",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "FavoriteRecipes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteRecipes_Users_UserId",
                table: "FavoriteRecipes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MealPlanEntries_Users_UserId",
                table: "MealPlanEntries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PantryAdjustments_Users_UserId",
                table: "PantryAdjustments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PantryItems_Users_UserId",
                table: "PantryItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipePreparations_Users_UserId",
                table: "RecipePreparations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
