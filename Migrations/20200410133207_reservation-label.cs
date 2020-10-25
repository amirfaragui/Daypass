using Microsoft.EntityFrameworkCore.Migrations;

namespace ValueCards.Migrations
{
    public partial class reservationlabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassPhase",
                table: "Tenants");

            migrationBuilder.AddColumn<string>(
                name: "PassPhrase",
                table: "Tenants",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReservationLabel",
                table: "Tenants",
                maxLength: 48,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassPhrase",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "ReservationLabel",
                table: "Tenants");

            migrationBuilder.AddColumn<string>(
                name: "PassPhase",
                table: "Tenants",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
