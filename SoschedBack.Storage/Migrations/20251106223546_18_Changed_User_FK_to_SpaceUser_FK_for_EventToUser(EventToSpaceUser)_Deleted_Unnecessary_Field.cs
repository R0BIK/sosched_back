using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoschedBack.Storage.Migrations
{
    /// <inheritdoc />
    public partial class _18_Changed_User_FK_to_SpaceUser_FK_for_EventToUserEventToSpaceUser_Deleted_Unnecessary_Field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventDateId",
                table: "Events");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventDateId",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
