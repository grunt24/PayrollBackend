namespace PayrollSystemBackend.Core.Dto.Payroll.PayslipPerPositionAndDepartment;

public class DepartmentPayrollDto
{
    public required string DepartmentName { get; set; }
    public required List<PayslipDto> Payrolls { get; set; } = new List<PayslipDto>();
}
