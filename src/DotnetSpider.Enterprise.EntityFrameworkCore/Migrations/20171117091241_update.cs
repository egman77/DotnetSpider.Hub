using Microsoft.EntityFrameworkCore.Migrations;

namespace DotnetSpider.Enterprise.EntityFrameworkCore.Migrations
{
	public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSingle",
                table: "Task",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSingle",
                table: "Task");
        }
    }
}
