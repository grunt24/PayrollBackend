using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollSystemBackend.Migrations
{
    /// <inheritdoc />
    public partial class updateLeaveClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeaveEndDate",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "LeaveStartDate",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Leaves");

            migrationBuilder.AddColumn<string>(
                name: "LeaveDates",
                table: "Leaves",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeaveDates",
                table: "Leaves");

            migrationBuilder.AddColumn<DateTime>(
                name: "LeaveEndDate",
                table: "Leaves",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LeaveStartDate",
                table: "Leaves",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Leaves",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
