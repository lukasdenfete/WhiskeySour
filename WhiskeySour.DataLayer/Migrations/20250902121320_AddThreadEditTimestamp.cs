using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhiskeySour.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddThreadEditTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EditedAt",
                table: "Threads",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EditedAt",
                table: "Threads");
        }
    }
}
