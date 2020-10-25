using Microsoft.EntityFrameworkCore.Migrations;

namespace ValueCards.Migrations
{
    public partial class passphase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PassPhrase",
                table: "Tenants",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QRCode",
                table: "Reservations",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassPhrase",
                table: "Tenants");

            migrationBuilder.AlterColumn<string>(
                name: "QRCode",
                table: "Reservations",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);
        }
    }
}
