using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CulinaryCommand.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRecipeAllergens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Allergens",
                table: "Recipes");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Vendors",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "EmailConfirmed",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "TaskTemplates",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "TaskLists",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSubRecipe",
                table: "Recipes",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLocationLocked",
                table: "PurchaseOrders",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLowStock",
                table: "InventoryItemDTO",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "IsActive",
                table: "Vendors",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<ulong>(
                name: "IsActive",
                table: "Users",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<ulong>(
                name: "EmailConfirmed",
                table: "Users",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<ulong>(
                name: "IsActive",
                table: "TaskTemplates",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<ulong>(
                name: "IsActive",
                table: "TaskLists",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<ulong>(
                name: "IsSubRecipe",
                table: "Recipes",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AddColumn<string>(
                name: "Allergens",
                table: "Recipes",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "IsLocationLocked",
                table: "PurchaseOrders",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<ulong>(
                name: "IsLowStock",
                table: "InventoryItemDTO",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");
        }
    }
}
