using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SoschedBack.Storage.Migrations
{
    /// <inheritdoc />
    public partial class _07_Fixed_Navigation_Properties_Fixed_Space_Intagration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventToUsers_SpaceUser_SpaceUserId",
                table: "EventToUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TagToUsers_SpaceUser_SpaceUserId",
                table: "TagToUsers");

            migrationBuilder.DropTable(
                name: "SpaceUser");

            migrationBuilder.DropIndex(
                name: "IX_PermissionToRoles_PermissionId",
                table: "PermissionToRoles");

            migrationBuilder.RenameColumn(
                name: "SpaceUserId",
                table: "TagToUsers",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TagToUsers_SpaceUserId",
                table: "TagToUsers",
                newName: "IX_TagToUsers_UserId");

            migrationBuilder.RenameColumn(
                name: "SpaceUserId",
                table: "EventToUsers",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventToUsers_SpaceUserId",
                table: "EventToUsers",
                newName: "IX_EventToUsers_UserId");

            migrationBuilder.AddColumn<int>(
                name: "SpaceId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpaceId",
                table: "Tags",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpaceId",
                table: "TagTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpaceId",
                table: "Roles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpaceId",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Space",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<string>(type: "text", nullable: false),
                    Website = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Space", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_SpaceId",
                table: "Users",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_SpaceId",
                table: "Tags",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TagTypes_SpaceId",
                table: "TagTypes",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_SpaceId",
                table: "Roles",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionToRoles_PermissionId",
                table: "PermissionToRoles",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_SpaceId",
                table: "Events",
                column: "SpaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventToUsers_Users_UserId",
                table: "EventToUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_TagToUsers_Users_UserId",
                table: "TagToUsers",
                column: "UserId",
                principalTable: "Users",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventToUsers_Users_UserId",
                table: "EventToUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Space_SpaceId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Space_SpaceId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_TagToUsers_Users_UserId",
                table: "TagToUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TagTypes_Space_SpaceId",
                table: "TagTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Space_SpaceId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Space_SpaceId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Space");

            migrationBuilder.DropIndex(
                name: "IX_Users_SpaceId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Tags_SpaceId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_TagTypes_SpaceId",
                table: "TagTypes");

            migrationBuilder.DropIndex(
                name: "IX_Roles_SpaceId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_PermissionToRoles_PermissionId",
                table: "PermissionToRoles");

            migrationBuilder.DropIndex(
                name: "IX_Events_SpaceId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "SpaceId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SpaceId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "SpaceId",
                table: "TagTypes");

            migrationBuilder.DropColumn(
                name: "SpaceId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "SpaceId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TagToUsers",
                newName: "SpaceUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TagToUsers_UserId",
                table: "TagToUsers",
                newName: "IX_TagToUsers_SpaceUserId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "EventToUsers",
                newName: "SpaceUserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventToUsers_UserId",
                table: "EventToUsers",
                newName: "IX_EventToUsers_SpaceUserId");

            migrationBuilder.CreateTable(
                name: "SpaceUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
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
                name: "IX_PermissionToRoles_PermissionId",
                table: "PermissionToRoles",
                column: "PermissionId",
                unique: true);

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
                name: "FK_EventToUsers_SpaceUser_SpaceUserId",
                table: "EventToUsers",
                column: "SpaceUserId",
                principalTable: "SpaceUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagToUsers_SpaceUser_SpaceUserId",
                table: "TagToUsers",
                column: "SpaceUserId",
                principalTable: "SpaceUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
