using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Inside.Web.Migrations
{
    public partial class ChangeParkingCategoryEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "ParkingCategories",
                newName: "MonthPrice");

            migrationBuilder.RenameColumn(
                name: "CoinPrice",
                table: "ParkingCategories",
                newName: "HourPrice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MonthPrice",
                table: "ParkingCategories",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "HourPrice",
                table: "ParkingCategories",
                newName: "CoinPrice");
        }
    }
}
