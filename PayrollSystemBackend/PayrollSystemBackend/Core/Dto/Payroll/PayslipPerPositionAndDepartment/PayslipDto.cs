using PayrollSystemBackend.Core.Dto.Payroll.EmployeeAllowancesDto;
using PayrollSystemBackend.Core.Entities;

namespace PayrollSystemBackend.Core.Dto.Payroll.PayslipPerPositionAndDepartment;

public class PayslipDto
{
    public required int PayrollNumber { get; set; }
    public required string IdNumber { get; set; }
    public required string Department { get; set; }
    public required string Position { get; set; }
    public required string FullName { get; set; }
    public required string PayrollStartDate { get; set; }
    public required string PayrollEndDate { get; set; }
    public required decimal GrossSalary { get; set; }
    public required decimal NetSalary { get; set; }
    public required decimal TotalDeductions { get; set; }

    public required decimal SssEmployeeShare { get; set; }
    public required decimal SssEmployerShare { get; set; }

    public required decimal PhilHealthEmployeeShare { get; set; }
    public required decimal PhilHealthEmployerShare { get; set; }

    public required decimal PagibigEmployeeShare { get; set; }
    public required decimal PagibigEmployerShare { get; set; }

    public required decimal TotalEmployeeContributions { get; set; }
    public required decimal TotalEmployerContributions { get; set; }
    public required decimal TotalContribution { get; set; }

    public required List<EmployeeDeduction> EmployeeDeductions { get; set; } = [];
}
