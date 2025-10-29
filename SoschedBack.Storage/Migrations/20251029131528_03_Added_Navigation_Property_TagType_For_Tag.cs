using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoschedBack.Storage.Migrations
{
    /// <inheritdoc />
    public partial class _03_Added_Navigation_Property_TagType_For_Tag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagTypeId",
                table: "Tags",
                column: "TagTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_TagTypes_TagTypeId",
                table: "Tags",
                column: "TagTypeId",
                principalTable: "TagTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_TagTypes_TagTypeId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TagTypeId",
                table: "Tags");
        }
    }
}
