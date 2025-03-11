using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPSLSGame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameGameResultColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "GameResults");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "GameResults",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "GameResults");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "GameResults",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
