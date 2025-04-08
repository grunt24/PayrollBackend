using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollSystemBackend.Migrations
{
    /// <inheritdoc />
    public partial class employeeLeaves : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LeaveDates",
                table: "Payrolls",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeaveDates",
                table: "Payrolls");
        }
    }
}
