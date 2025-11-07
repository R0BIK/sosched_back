using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoschedBack.Storage.Migrations
{
    /// <inheritdoc />
    public partial class _19_Fix_Rename_EventToSpaceUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventToUsers_Events_EventId",
                table: "EventToUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_EventToUsers_Users_UserId",
                table: "EventToUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventToUsers",
                table: "EventToUsers");

            migrationBuilder.RenameTable(
                name: "EventToUsers",
                newName: "EventToSpaceUsers");

            migrationBuilder.RenameIndex(
                name: "IX_EventToUsers_UserId",
                table: "EventToSpaceUsers",
                newName: "IX_EventToSpaceUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventToUsers_EventId",
                table: "EventToSpaceUsers",
                newName: "IX_EventToSpaceUsers_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventToSpaceUsers",
                table: "EventToSpaceUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventToSpaceUsers_Events_EventId",
                table: "EventToSpaceUsers",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventToSpaceUsers_Users_UserId",
                table: "EventToSpaceUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventToSpaceUsers_Events_EventId",
                table: "EventToSpaceUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_EventToSpaceUsers_Users_UserId",
                table: "EventToSpaceUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventToSpaceUsers",
                table: "EventToSpaceUsers");

            migrationBuilder.RenameTable(
                name: "EventToSpaceUsers",
                newName: "EventToUsers");

            migrationBuilder.RenameIndex(
                name: "IX_EventToSpaceUsers_UserId",
                table: "EventToUsers",
                newName: "IX_EventToUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventToSpaceUsers_EventId",
                table: "EventToUsers",
                newName: "IX_EventToUsers_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventToUsers",
                table: "EventToUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventToUsers_Events_EventId",
                table: "EventToUsers",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventToUsers_Users_UserId",
                table: "EventToUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
