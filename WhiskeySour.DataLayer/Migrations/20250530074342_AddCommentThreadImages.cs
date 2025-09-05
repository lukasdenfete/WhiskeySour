using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhiskeySour.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentThreadImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Threads",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Comments",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Comments");
        }
    }
}
