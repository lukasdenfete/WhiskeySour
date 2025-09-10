using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhiskeySour.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateTable(
        name: "Notifications",
        columns: table => new
        {
            Id = table.Column<int>(type: "int", nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
            UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Type = table.Column<int>(type: "int", nullable: false),
            FromUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
            ThreadId = table.Column<int>(type: "int", nullable: true),
            CommentId = table.Column<int>(type: "int", nullable: true),
            MessageId = table.Column<int>(type: "int", nullable: true),
            isRead = table.Column<bool>(type: "bit", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Notifications", x => x.Id);
            table.ForeignKey(
                name: "FK_Notifications_AspNetUsers_FromUserId",
                column: x => x.FromUserId,
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                column: x => x.UserId,
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                name: "FK_Notifications_Comments_CommentId",
                column: x => x.CommentId,
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                name: "FK_Notifications_Messages_MessageId",
                column: x => x.MessageId,
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                name: "FK_Notifications_Threads_ThreadId",
                column: x => x.ThreadId,
                principalTable: "Threads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        });

    migrationBuilder.CreateIndex(
        name: "IX_Notifications_CommentId",
        table: "Notifications",
        column: "CommentId");

    migrationBuilder.CreateIndex(
        name: "IX_Notifications_FromUserId",
        table: "Notifications",
        column: "FromUserId");

    migrationBuilder.CreateIndex(
        name: "IX_Notifications_MessageId",
        table: "Notifications",
        column: "MessageId");

    migrationBuilder.CreateIndex(
        name: "IX_Notifications_ThreadId",
        table: "Notifications",
        column: "ThreadId");

    migrationBuilder.CreateIndex(
        name: "IX_Notifications_UserId",
        table: "Notifications",
        column: "UserId");
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");
        }
    }
}
