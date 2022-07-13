using Microsoft.EntityFrameworkCore.Migrations;

namespace YAHGA_Server.Migrations
{
    public partial class RemoveTestField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Test",
                table: "PublicEntities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Test",
                table: "PublicEntities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
