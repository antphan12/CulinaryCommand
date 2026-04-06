using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CulinaryCommand.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
