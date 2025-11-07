using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoschedBack.Storage.Migrations
{
    /// <inheritdoc />
    public partial class _10_Deleted_Useless_Fields_In_Space : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Space");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Space");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Space");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Space");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Space");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Space",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Space",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Space",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Space",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Space",
                type: "text",
                nullable: true);
        }
    }
}
