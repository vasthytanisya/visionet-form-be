using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Visionet.Form.Entities.Migrations
{
    public partial class AddTotalGrocery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalGrocery",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalGrocery",
                table: "Transactions");
        }
    }
}
