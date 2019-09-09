using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShutdownTimer.Server.Data.Migrations
{
    public partial class ShutdownHistoryFeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShutdownExecutionHotlinks",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    Hours = table.Column<int>(nullable: false),
                    Minutes = table.Column<int>(nullable: false),
                    LastExecution = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShutdownExecutionHotlinks", x => new { x.UserId, x.Hours, x.Minutes });
                    table.ForeignKey(
                        name: "FK_ShutdownExecutionHotlinks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShutdownExecutionHotlinks");
        }
    }
}
