using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollSystemBackend.Migrations
{
    /// <inheritdoc />
    public partial class add_SSSContribution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SSSContributions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinCompensation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxCompensation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployerSS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployerEC = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployerMPF = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployeeSS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployeeMPF = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SSSContributions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SSSContributions");
        }
    }
}
