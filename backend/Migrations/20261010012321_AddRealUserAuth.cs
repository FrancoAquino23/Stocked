using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stocked.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRealUserAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PantryItems_IngredientId",
                table: "PantryItems");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "Users",
                newName: "Username");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "RecipePreparations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PantryItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PantryAdjustments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "MealPlanEntries",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "FavoriteRecipes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipePreparations_UserId",
                table: "RecipePreparations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PantryItems_IngredientId",
                table: "PantryItems",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_PantryItems_UserId_IngredientId",
                table: "PantryItems",
                columns: new[] { "UserId", "IngredientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PantryAdjustments_UserId",
                table: "PantryAdjustments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanEntries_UserId",
                table: "MealPlanEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteRecipes_UserId",
                table: "FavoriteRecipes",
                column: "UserId");

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

            migrationBuilder.DropIndex(
                name: "IX_RecipePreparations_UserId",
                table: "RecipePreparations");

            migrationBuilder.DropIndex(
                name: "IX_PantryItems_IngredientId",
                table: "PantryItems");

            migrationBuilder.DropIndex(
                name: "IX_PantryItems_UserId_IngredientId",
                table: "PantryItems");

            migrationBuilder.DropIndex(
                name: "IX_PantryAdjustments_UserId",
                table: "PantryAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_MealPlanEntries_UserId",
                table: "MealPlanEntries");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteRecipes_UserId",
                table: "FavoriteRecipes");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RecipePreparations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PantryItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PantryAdjustments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MealPlanEntries");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FavoriteRecipes");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "DisplayName");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "DisplayName", "Email" },
                values: new object[] { 1, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Usuario de Prueba", "prueba@stocked.dev" });

            migrationBuilder.CreateIndex(
                name: "IX_PantryItems_IngredientId",
                table: "PantryItems",
                column: "IngredientId",
                unique: true);
        }
    }
}
