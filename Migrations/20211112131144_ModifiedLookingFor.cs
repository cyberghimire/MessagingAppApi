using Microsoft.EntityFrameworkCore.Migrations;

namespace MessagingApp.API.Migrations
{
    public partial class ModifiedLookingFor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LookinFor",
                table: "Users",
                newName: "LookingFor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LookingFor",
                table: "Users",
                newName: "LookinFor");
        }
    }
}
