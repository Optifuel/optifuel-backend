using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiCos.Migrations
{
    /// <inheritdoc />
    public partial class AddChangePassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChangePassword",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<int>(type: "integer", nullable: false),
                    DeadLine = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NewPasswordSalt = table.Column<byte[]>(type: "bytea", nullable: false),
                    NewPasswordHash = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangePassword", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_ChangePassword_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChangePassword");
        }
    }
}
