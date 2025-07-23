using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaKing.BackendServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class _14072025_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Versions_SizeId",
                table: "ProductVariants");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "Versions",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SizeId",
                table: "ProductVariants",
                newName: "VersionId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVariants_SizeId",
                table: "ProductVariants",
                newName: "IX_ProductVariants_VersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Versions_VersionId",
                table: "ProductVariants",
                column: "VersionId",
                principalTable: "Versions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Versions_VersionId",
                table: "ProductVariants");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Versions",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "VersionId",
                table: "ProductVariants",
                newName: "SizeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVariants_VersionId",
                table: "ProductVariants",
                newName: "IX_ProductVariants_SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Versions_SizeId",
                table: "ProductVariants",
                column: "SizeId",
                principalTable: "Versions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
