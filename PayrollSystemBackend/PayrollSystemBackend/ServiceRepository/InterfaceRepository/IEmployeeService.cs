using Microsoft.AspNetCore.Mvc;
using payroll_system.Core.Entities;
using PayrollSystem.Domain.Entities;
using PayrollSystemBackend.Core.Entities;
using System.Collections.Generic;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface IEmployeeService
    {
        string AddEmployee(Employee employee);
        bool DeleteEmployee(int id);
        IEnumerable<Employee> GetAllEmployee();
        int CountEmployee();
        bool AddEmployeeDeductions(List<EmployeeDeduction> deductions);
        bool AddEmployeeAllowances(List<Allowance> additionalEarnings);
        Task<Employee?> GetEmployeeByIdAsync(int employeeId);

    }
}
