using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollSystemBackend.Migrations
{
    /// <inheritdoc />
    public partial class modifyLeaves : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdNumber",
                table: "Leaves",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdNumber",
                table: "Leaves");
        }
    }
}
