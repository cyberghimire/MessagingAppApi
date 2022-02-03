using Microsoft.EntityFrameworkCore.Migrations;

namespace MessagingApp.API.Migrations
{
    public partial class MessageEntityModifiedId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Messages",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Messages",
                newName: "id");
        }
    }
}
