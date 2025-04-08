namespace PayrollSystemBackend.Core.Dto.Payroll.PayslipPerPositionAndDepartment
{
    public class EmployeePayrollDto
    {
        public required List<PayslipDto> Payrolls { get; set; } = new List<PayslipDto>();
    }
}
