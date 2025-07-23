using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaKing.BackendServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class _18072025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Versions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Versions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Versions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ColorModelId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VersionModelId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Colors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Colors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Colors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Dob",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ColorModelId",
                table: "Products",
                column: "ColorModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_VersionModelId",
                table: "Products",
                column: "VersionModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Colors_ColorModelId",
                table: "Products",
                column: "ColorModelId",
                principalTable: "Colors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Versions_VersionModelId",
                table: "Products",
                column: "VersionModelId",
                principalTable: "Versions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Colors_ColorModelId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Versions_VersionModelId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ColorModelId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_VersionModelId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Versions");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Versions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Versions");

            migrationBuilder.DropColumn(
                name: "ColorModelId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VersionModelId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "Dob",
                table: "AspNetUsers");
        }
    }
}
