using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaKing.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSKUToCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SKU",
                table: "AppProducts",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "SKU",
                table: "AppOrderItems",
                newName: "ProductCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Code",
                table: "AppProducts",
                newName: "SKU");

            migrationBuilder.RenameColumn(
                name: "ProductCode",
                table: "AppOrderItems",
                newName: "SKU");
        }
    }
}
