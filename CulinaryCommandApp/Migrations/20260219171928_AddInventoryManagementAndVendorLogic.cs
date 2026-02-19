using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CulinaryCommand.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryManagementAndVendorLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "Ingredients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_LocationId",
                table: "Ingredients",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_VendorId",
                table: "Ingredients",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Locations_LocationId",
                table: "Ingredients",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Vendors_VendorId",
                table: "Ingredients",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Locations_LocationId",
                table: "Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Vendors_VendorId",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_LocationId",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_VendorId",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Ingredients");
        }
    }
}
