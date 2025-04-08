namespace PayrollSystemBackend.Core.Dto.Payroll.PayslipPerPositionAndDepartment;

public class PositionPayrollDto
{
    public required string PositionName { get; set; }
    public required List<PayslipDto> Payrolls { get; set; } = new List<PayslipDto>();
}
