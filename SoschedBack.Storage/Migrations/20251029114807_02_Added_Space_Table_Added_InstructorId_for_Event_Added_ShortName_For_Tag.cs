using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoschedBack.Storage.Migrations
{
    /// <inheritdoc />
    public partial class _02_Added_Space_Table_Added_InstructorId_for_Event_Added_ShortName_For_Tag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "Tags",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "InstructirId",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "InstructirId",
                table: "Events");
        }
    }
}
