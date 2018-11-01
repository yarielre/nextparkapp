using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Inside.Data.Migrations
{
    public partial class ChangeEventParkingRealationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Parkings_ParkingEventId",
                table: "Parkings");

            migrationBuilder.DropColumn(
                name: "ParkingId",
                table: "Events");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "Events",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "Events",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.CreateIndex(
                name: "IX_Parkings_ParkingEventId",
                table: "Parkings",
                column: "ParkingEventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Parkings_ParkingEventId",
                table: "Parkings");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Events",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "Events",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AddColumn<int>(
                name: "ParkingId",
                table: "Events",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Parkings_ParkingEventId",
                table: "Parkings",
                column: "ParkingEventId",
                unique: true);
        }
    }
}
