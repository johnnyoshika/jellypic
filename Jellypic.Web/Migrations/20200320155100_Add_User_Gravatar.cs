using Microsoft.EntityFrameworkCore.Migrations;

namespace Jellypic.Web.Migrations
{
    public partial class Add_User_Gravatar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gravatar",
                table: "Users",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gravatar",
                table: "Users");
        }
    }
}
