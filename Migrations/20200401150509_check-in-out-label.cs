using Microsoft.EntityFrameworkCore.Migrations;

namespace ValueCards.Migrations
{
    public partial class checkinoutlabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckInLabel",
                table: "Tenants",
                maxLength: 48,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckOutLabel",
                table: "Tenants",
                maxLength: 48,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInLabel",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "CheckOutLabel",
                table: "Tenants");
        }
    }
}
