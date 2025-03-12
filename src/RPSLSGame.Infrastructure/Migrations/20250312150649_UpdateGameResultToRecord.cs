using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPSLSGame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGameResultToRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Player",
                table: "GameResults",
                newName: "PlayerMove");

            migrationBuilder.RenameColumn(
                name: "Computer",
                table: "GameResults",
                newName: "ComputerMove");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "GameResults",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlayerMove",
                table: "GameResults",
                newName: "Player");

            migrationBuilder.RenameColumn(
                name: "ComputerMove",
                table: "GameResults",
                newName: "Computer");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "GameResults",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
