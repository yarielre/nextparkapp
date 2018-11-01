using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Inside.Web.Migrations
{
    public partial class AddindEndDatetoOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Orders",
                newName: "StartDate");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Orders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Orders",
                newName: "Date");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
