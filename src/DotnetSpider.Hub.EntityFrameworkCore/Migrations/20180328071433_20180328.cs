using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DotnetSpider.Hub.EntityFrameworkCore.Migrations
{
	public partial class _20180328 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskLog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Exception = table.Column<string>(nullable: true),
                    Identity = table.Column<string>(maxLength: 32, nullable: false),
                    Level = table.Column<string>(nullable: true),
                    Logged = table.Column<DateTime>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    NodeId = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskLog", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskLog");
        }
    }
}
