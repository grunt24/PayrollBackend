using PayrollSystem.Domain.Entities;
using PayrollSystemBackend.Core.Dto.Payroll;
using PayrollSystemBackend.Core.Dto.Payroll.PayslipPerPositionAndDepartment;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface IPayrollService
    {
        Task<List<PayrollResult>> GeneratePayroll(PayrollAddRequestDto requestDto);
        Payroll GetPayrollById(int payrollId);
        IEnumerable<PayrollDto> GetAllPayrolls();
        (decimal TotalNetSalary, decimal TotalDeductions, decimal TotalGrossIncome) GetPayrollSummary();
        Task<Payroll?> UpdatePayrollStatusAsync(PayrollStatusRequest request);
        Task<PayrollStatusCountDto> GetPayrollStatusCount();
        Task<bool> DeletePayroll(int id);
        Task<IEnumerable<DepartmentPayrollDto>> GetPayrollsByDepartment();
        Task<IEnumerable<PositionPayrollDto>> GetPayrollsByPosition();
        Task<IEnumerable<PayslipDto>> GetPayrollsByEmployee(int employeeId);
        Task<IEnumerable<PayslipDto>> GetPayslipByPayrollId(int payrollId);
        Task<IEnumerable<PayslipDto>> GetPayrolls();


    }
}
