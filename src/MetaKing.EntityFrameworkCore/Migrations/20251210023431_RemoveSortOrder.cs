using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaKing.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSortOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "AppProducts");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "AppProductCategories");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "AppProductAttributes");

            migrationBuilder.RenameColumn(
                name: "Visibility",
                table: "AppProducts",
                newName: "IsVisibility");

            migrationBuilder.RenameColumn(
                name: "Visibility",
                table: "AppProductCategories",
                newName: "IsVisibility");

            migrationBuilder.RenameColumn(
                name: "Visibility",
                table: "AppProductAttributes",
                newName: "IsVisibility");

            migrationBuilder.RenameColumn(
                name: "Visibility",
                table: "AppManufacturers",
                newName: "IsVisibility");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVisibility",
                table: "AppProducts",
                newName: "Visibility");

            migrationBuilder.RenameColumn(
                name: "IsVisibility",
                table: "AppProductCategories",
                newName: "Visibility");

            migrationBuilder.RenameColumn(
                name: "IsVisibility",
                table: "AppProductAttributes",
                newName: "Visibility");

            migrationBuilder.RenameColumn(
                name: "IsVisibility",
                table: "AppManufacturers",
                newName: "Visibility");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "AppProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "AppProductCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "AppProductAttributes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
