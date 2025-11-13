using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SoschedBack.Storage.Migrations
{
    /// <inheritdoc />
    public partial class _20_Fixed_Connection_TagToSpaceUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventToSpaceUsers_Users_UserId",
                table: "EventToSpaceUsers");

            migrationBuilder.DropTable(
                name: "TagToUsers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "EventToSpaceUsers",
                newName: "SpaceUserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventToSpaceUsers_UserId",
                table: "EventToSpaceUsers",
                newName: "IX_EventToSpaceUsers_SpaceUserId");

            migrationBuilder.CreateTable(
                name: "TagToSpaceUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SpaceUserId = table.Column<int>(type: "integer", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagToSpaceUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagToSpaceUsers_SpaceUsers_SpaceUserId",
                        column: x => x.SpaceUserId,
                        principalTable: "SpaceUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagToSpaceUsers_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagToSpaceUsers_SpaceUserId",
                table: "TagToSpaceUsers",
                column: "SpaceUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TagToSpaceUsers_TagId",
                table: "TagToSpaceUsers",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventToSpaceUsers_SpaceUsers_SpaceUserId",
                table: "EventToSpaceUsers",
                column: "SpaceUserId",
                principalTable: "SpaceUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventToSpaceUsers_SpaceUsers_SpaceUserId",
                table: "EventToSpaceUsers");

            migrationBuilder.DropTable(
                name: "TagToSpaceUsers");

            migrationBuilder.RenameColumn(
                name: "SpaceUserId",
                table: "EventToSpaceUsers",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventToSpaceUsers_SpaceUserId",
                table: "EventToSpaceUsers",
                newName: "IX_EventToSpaceUsers_UserId");

            migrationBuilder.CreateTable(
                name: "TagToUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TagId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagToUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagToUsers_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagToUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagToUsers_TagId",
                table: "TagToUsers",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TagToUsers_UserId",
                table: "TagToUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventToSpaceUsers_Users_UserId",
                table: "EventToSpaceUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
