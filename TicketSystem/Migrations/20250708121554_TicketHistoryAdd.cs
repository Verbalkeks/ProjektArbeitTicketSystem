using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSystem.Migrations
{
    /// <inheritdoc />
    public partial class TicketHistoryAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tickets_TicketId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_MsgUsers_AspNetUsers_UserId1",
                table: "MsgUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_MsgUsers_AspNetUsers_UserId2",
                table: "MsgUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_CreatorId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Projects_AssignedProjectId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_MsgUsers_UserId1",
                table: "MsgUsers");

            migrationBuilder.DropIndex(
                name: "IX_MsgUsers_UserId2",
                table: "MsgUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId2",
                table: "MsgUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId1",
                table: "MsgUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "User1Id",
                table: "MsgUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "User2Id",
                table: "MsgUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TicketAssignHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssignedToId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TicketId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAssignHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketAssignHistories_AspNetUsers_AssignedById",
                        column: x => x.AssignedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketAssignHistories_AspNetUsers_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MsgUsers_User1Id",
                table: "MsgUsers",
                column: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_MsgUsers_User2Id",
                table: "MsgUsers",
                column: "User2Id");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAssignHistories_AssignedById",
                table: "TicketAssignHistories",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAssignHistories_AssignedToId",
                table: "TicketAssignHistories",
                column: "AssignedToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tickets_TicketId",
                table: "Comments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MsgUsers_AspNetUsers_User1Id",
                table: "MsgUsers",
                column: "User1Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MsgUsers_AspNetUsers_User2Id",
                table: "MsgUsers",
                column: "User2Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_CreatorId",
                table: "Tickets",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Projects_AssignedProjectId",
                table: "Tickets",
                column: "AssignedProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tickets_TicketId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_MsgUsers_AspNetUsers_User1Id",
                table: "MsgUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_MsgUsers_AspNetUsers_User2Id",
                table: "MsgUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_CreatorId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Projects_AssignedProjectId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "TicketAssignHistories");

            migrationBuilder.DropIndex(
                name: "IX_MsgUsers_User1Id",
                table: "MsgUsers");

            migrationBuilder.DropIndex(
                name: "IX_MsgUsers_User2Id",
                table: "MsgUsers");

            migrationBuilder.DropColumn(
                name: "User1Id",
                table: "MsgUsers");

            migrationBuilder.DropColumn(
                name: "User2Id",
                table: "MsgUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId2",
                table: "MsgUsers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId1",
                table: "MsgUsers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_MsgUsers_UserId1",
                table: "MsgUsers",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_MsgUsers_UserId2",
                table: "MsgUsers",
                column: "UserId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tickets_TicketId",
                table: "Comments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MsgUsers_AspNetUsers_UserId1",
                table: "MsgUsers",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MsgUsers_AspNetUsers_UserId2",
                table: "MsgUsers",
                column: "UserId2",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_CreatorId",
                table: "Tickets",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Projects_AssignedProjectId",
                table: "Tickets",
                column: "AssignedProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
