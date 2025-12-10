using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaKing.Migrations
{
    /// <inheritdoc />
    public partial class DeleteSeoMetaProductCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeoMetaDescription",
                table: "AppProductCategories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeoMetaDescription",
                table: "AppProductCategories",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}
