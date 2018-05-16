using Microsoft.EntityFrameworkCore.Migrations;

namespace DotnetSpider.Enterprise.EntityFrameworkCore.Migrations
{
	public partial class _20180209 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CPULoad",
                table: "NodeHeartbeat",
                newName: "CpuLoad");

            migrationBuilder.RenameColumn(
                name: "CPUCoreCount",
                table: "NodeHeartbeat",
                newName: "CpuCoreCount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CpuLoad",
                table: "NodeHeartbeat",
                newName: "CPULoad");

            migrationBuilder.RenameColumn(
                name: "CpuCoreCount",
                table: "NodeHeartbeat",
                newName: "CPUCoreCount");
        }
    }
}
