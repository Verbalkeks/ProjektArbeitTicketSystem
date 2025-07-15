using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSystem.Migrations
{
    /// <inheritdoc />
    public partial class TickNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tickets_TicketId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Dependencies_Tickets_DependentTicketId",
                table: "Dependencies");

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

            migrationBuilder.AlterColumn<int>(
                name: "AssignedProjectId",
                table: "Tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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
                name: "FK_Dependencies_Tickets_DependentTicketId",
                table: "Dependencies",
                column: "DependentTicketId",
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
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tickets_TicketId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Dependencies_Tickets_DependentTicketId",
                table: "Dependencies");

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

            migrationBuilder.AlterColumn<int>(
                name: "AssignedProjectId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_MsgUsers_User1Id",
                table: "MsgUsers",
                column: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_MsgUsers_User2Id",
                table: "MsgUsers",
                column: "User2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tickets_TicketId",
                table: "Comments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dependencies_Tickets_DependentTicketId",
                table: "Dependencies",
                column: "DependentTicketId",
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
    }
}
