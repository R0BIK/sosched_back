using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoschedBack.Storage.Migrations
{
    /// <inheritdoc />
    public partial class _11_DbSet_Space : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Space_SpaceId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Space_SpaceId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_TagTypes_Space_SpaceId",
                table: "TagTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Space_SpaceId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Space_SpaceId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Space",
                table: "Space");

            migrationBuilder.RenameTable(
                name: "Space",
                newName: "Spaces");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Spaces",
                table: "Spaces",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Spaces_SpaceId",
                table: "Events",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Spaces_SpaceId",
                table: "Roles",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagTypes_Spaces_SpaceId",
                table: "TagTypes",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Spaces_SpaceId",
                table: "Tags",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Spaces_SpaceId",
                table: "Users",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Spaces_SpaceId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Spaces_SpaceId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_TagTypes_Spaces_SpaceId",
                table: "TagTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Spaces_SpaceId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Spaces_SpaceId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Spaces",
                table: "Spaces");

            migrationBuilder.RenameTable(
                name: "Spaces",
                newName: "Space");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Space",
                table: "Space",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Space_SpaceId",
                table: "Events",
                column: "SpaceId",
                principalTable: "Space",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Space_SpaceId",
                table: "Roles",
                column: "SpaceId",
                principalTable: "Space",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagTypes_Space_SpaceId",
                table: "TagTypes",
                column: "SpaceId",
                principalTable: "Space",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Space_SpaceId",
                table: "Tags",
                column: "SpaceId",
                principalTable: "Space",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Space_SpaceId",
                table: "Users",
                column: "SpaceId",
                principalTable: "Space",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
