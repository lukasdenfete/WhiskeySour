using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhiskeySour.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationThreadCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Threads_ThreadId1",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ThreadId1",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ThreadId1",
                table: "Notifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThreadId1",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ThreadId1",
                table: "Notifications",
                column: "ThreadId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Threads_ThreadId1",
                table: "Notifications",
                column: "ThreadId1",
                principalTable: "Threads",
                principalColumn: "Id");
        }
    }
}
