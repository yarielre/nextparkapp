using Microsoft.EntityFrameworkCore.Migrations;

namespace NextPark.Api.Migrations
{
    public partial class ParkingNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Parkings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Parkings");
        }
    }
}
