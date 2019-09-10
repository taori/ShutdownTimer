using Microsoft.EntityFrameworkCore.Migrations;

namespace ShutdownTimer.Server.Data.Migrations
{
    public partial class CustomCommandAddCommandName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommandName",
                table: "CustomCommands",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommandName",
                table: "CustomCommands");
        }
    }
}
