using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NextPark.Api.Migrations
{
    public partial class UpdateModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Parkings_ParkingId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Parkings_ParkingCategories_ParkingCategoryId",
                table: "Parkings");

            migrationBuilder.DropForeignKey(
                name: "FK_Parkings_ParkingTypes_ParkingTypeId",
                table: "Parkings");

            migrationBuilder.DropTable(
                name: "ParkingCategories");

            migrationBuilder.DropTable(
                name: "ParkingTypes");

            migrationBuilder.DropIndex(
                name: "IX_Parkings_ParkingCategoryId",
                table: "Parkings");

            migrationBuilder.DropIndex(
                name: "IX_Parkings_ParkingTypeId",
                table: "Parkings");

            migrationBuilder.DropColumn(
                name: "ParkingCategoryId",
                table: "Parkings");

            migrationBuilder.DropColumn(
                name: "ParkingTypeId",
                table: "Parkings");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "WeekRepeat",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "Coins",
                table: "AspNetUsers",
                newName: "Profit");

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Parkings",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Parkings",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cap",
                table: "Parkings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Parkings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentCode",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ParkingId",
                table: "Events",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RepetitionEndDate",
                table: "Events",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "RepetitionId",
                table: "Events",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "RepetitionType",
                table: "Events",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Balance",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Cap",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Parkings_ParkingId",
                table: "Events",
                column: "ParkingId",
                principalTable: "Parkings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Parkings_ParkingId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Cap",
                table: "Parkings");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Parkings");

            migrationBuilder.DropColumn(
                name: "PaymentCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RepetitionEndDate",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "RepetitionId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "RepetitionType",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Cap",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "City",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Profit",
                table: "AspNetUsers",
                newName: "Coins");

            migrationBuilder.AlterColumn<string>(
                name: "Longitude",
                table: "Parkings",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<string>(
                name: "Latitude",
                table: "Parkings",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<int>(
                name: "ParkingCategoryId",
                table: "Parkings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParkingTypeId",
                table: "Parkings",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "Orders",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "Orders",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AlterColumn<int>(
                name: "ParkingId",
                table: "Events",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "Events",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "Events",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "WeekRepeat",
                table: "Events",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ParkingCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Category = table.Column<string>(nullable: true),
                    HourPrice = table.Column<double>(nullable: false),
                    MonthPrice = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParkingTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parkings_ParkingCategoryId",
                table: "Parkings",
                column: "ParkingCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Parkings_ParkingTypeId",
                table: "Parkings",
                column: "ParkingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Parkings_ParkingId",
                table: "Events",
                column: "ParkingId",
                principalTable: "Parkings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Parkings_ParkingCategories_ParkingCategoryId",
                table: "Parkings",
                column: "ParkingCategoryId",
                principalTable: "ParkingCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Parkings_ParkingTypes_ParkingTypeId",
                table: "Parkings",
                column: "ParkingTypeId",
                principalTable: "ParkingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
