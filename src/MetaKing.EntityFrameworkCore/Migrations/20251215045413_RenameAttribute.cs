using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaKing.Migrations
{
    /// <inheritdoc />
    public partial class RenameAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Label",
                table: "AppProductAttributes",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AppProductAttributes",
                newName: "Label");
        }
    }
}
