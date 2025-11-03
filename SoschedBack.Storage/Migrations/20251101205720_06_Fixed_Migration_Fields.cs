using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SoschedBack.Storage.Migrations
{
    /// <inheritdoc />
    public partial class _06_Fixed_Migration_Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_TagToUsers_TagToUserId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_TagToUsers_TagToUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TagToUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TagToUserId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TagToUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TagToUserId",
                table: "Tags");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TagToUsers",
                newName: "SpaceUserId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "EventToUsers",
                newName: "SpaceUserId");

            migrationBuilder.CreateTable(
                name: "SpaceUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpaceUser_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpaceUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagToUsers_SpaceUserId",
                table: "TagToUsers",
                column: "SpaceUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TagToUsers_TagId",
                table: "TagToUsers",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionToRoles_PermissionId",
                table: "PermissionToRoles",
                column: "PermissionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionToRoles_RoleId",
                table: "PermissionToRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_EventToUsers_EventId",
                table: "EventToUsers",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventToUsers_SpaceUserId",
                table: "EventToUsers",
                column: "SpaceUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SpaceUser_RoleId",
                table: "SpaceUser",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SpaceUser_UserId",
                table: "SpaceUser",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventToUsers_Events_EventId",
                table: "EventToUsers",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventToUsers_SpaceUser_SpaceUserId",
                table: "EventToUsers",
                column: "SpaceUserId",
                principalTable: "SpaceUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionToRoles_Permissions_PermissionId",
                table: "PermissionToRoles",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionToRoles_Roles_RoleId",
                table: "PermissionToRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagToUsers_SpaceUser_SpaceUserId",
                table: "TagToUsers",
                column: "SpaceUserId",
                principalTable: "SpaceUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagToUsers_Tags_TagId",
                table: "TagToUsers",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventToUsers_Events_EventId",
                table: "EventToUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_EventToUsers_SpaceUser_SpaceUserId",
                table: "EventToUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionToRoles_Permissions_PermissionId",
                table: "PermissionToRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionToRoles_Roles_RoleId",
                table: "PermissionToRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_TagToUsers_SpaceUser_SpaceUserId",
                table: "TagToUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TagToUsers_Tags_TagId",
                table: "TagToUsers");

            migrationBuilder.DropTable(
                name: "SpaceUser");

            migrationBuilder.DropIndex(
                name: "IX_TagToUsers_SpaceUserId",
                table: "TagToUsers");

            migrationBuilder.DropIndex(
                name: "IX_TagToUsers_TagId",
                table: "TagToUsers");

            migrationBuilder.DropIndex(
                name: "IX_PermissionToRoles_PermissionId",
                table: "PermissionToRoles");

            migrationBuilder.DropIndex(
                name: "IX_PermissionToRoles_RoleId",
                table: "PermissionToRoles");

            migrationBuilder.DropIndex(
                name: "IX_EventToUsers_EventId",
                table: "EventToUsers");

            migrationBuilder.DropIndex(
                name: "IX_EventToUsers_SpaceUserId",
                table: "EventToUsers");

            migrationBuilder.RenameColumn(
                name: "SpaceUserId",
                table: "TagToUsers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "SpaceUserId",
                table: "EventToUsers",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "TagToUserId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TagToUserId",
                table: "Tags",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TagToUserId",
                table: "Users",
                column: "TagToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagToUserId",
                table: "Tags",
                column: "TagToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_TagToUsers_TagToUserId",
                table: "Tags",
                column: "TagToUserId",
                principalTable: "TagToUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_TagToUsers_TagToUserId",
                table: "Users",
                column: "TagToUserId",
                principalTable: "TagToUsers",
                principalColumn: "Id");
        }
    }
}
