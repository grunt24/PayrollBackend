using PayrollSystemBackend.Core.Dto.Employee;
using PayrollSystemBackend.Core.Entities;
using System.Collections.Generic;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface IEmployeeDeduction
    {
        bool AddEmployeeDeduction(EmployeeDeduction deduction);
        void DeleteEmployeeDeduction(int id);
        IEnumerable<EmployeeDeduction> GetAllEmployeeDeductions();
        EmployeeDeduction GetEmployeeDeductionById(int id);
        void UpdateEmployeeDeduction(EmployeeDeductionRequest deduction, int id);
        IEnumerable<EmployeeDeduction> GetEmployeeDeductionByEmployeeId(int employeeId);
        void UpdateEmployeeDeductionById(int employeeId, int deductionId, EmployeeDeduction updatedDeduction);
        void SaveChanges();
    }
}
