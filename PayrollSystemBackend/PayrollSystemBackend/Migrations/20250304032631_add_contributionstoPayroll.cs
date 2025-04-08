using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollSystemBackend.Migrations
{
    /// <inheritdoc />
    public partial class add_contributionstoPayroll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PagibigEmployeeShare",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PagibigEmployerShare",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PhilHealthEmployeeShare",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PhilHealthEmployerShare",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SSSEmployeeShare",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SSSEmployerShare",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalContribution",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalEmployeeContributions",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalEmployerContributions",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PagibigEmployeeShare",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "PagibigEmployerShare",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "PhilHealthEmployeeShare",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "PhilHealthEmployerShare",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "SSSEmployeeShare",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "SSSEmployerShare",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "TotalContribution",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "TotalEmployeeContributions",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "TotalEmployerContributions",
                table: "Payrolls");
        }
    }
}
