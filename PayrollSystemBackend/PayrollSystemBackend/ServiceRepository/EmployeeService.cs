using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PayrollSystem.Domain.Entities;
using payroll_system.Core.Entities;
using PayrollSystem.DataAccessEFCore;
using PayrollSystemBackend.Core.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;

namespace payroll_system.Core.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int CountEmployee()
        {
            return _context.Set<Employee>().Count();
        }

        public string AddEmployee(Employee data)
        {
            if(_context.Employees.Any(i=>i.IDNumber == data.IDNumber))
            {
                return "Employee IDNumber already exists.";
            }

            var academicAwardId = data.AcademicAwardId != 0 && _context.AcademicAwards.Any(a => a.Id == data.AcademicAwardId) ? data.AcademicAwardId : (int?)null;
            var benefitId = data.BenefitId != 0 && _context.Benefits.Any(b => b.Id == data.BenefitId) ? data.BenefitId : (int?)null;
            var educationalDegreeId = data.EducationalDegreeId != 0 && _context.EducationalDegrees.Any(e => e.Id == data.EducationalDegreeId) ? data.EducationalDegreeId : (int?)null;
            var positionId = data.PositionId != 0 && _context.Positions.Any(p => p.Id == data.PositionId) ? data.PositionId : (int?)null;
            var departmentId = data.DepartmentId != 0 && _context.Departments.Any(d => d.Id == data.DepartmentId) ? data.DepartmentId : (int?)null;

            

            var newEmployee = new Employee
            {
                FirstName = data.FirstName,
                LastName = data.LastName,
                MiddleInitial = data.MiddleInitial,
                ContactNumber = data.ContactNumber,
                BasicSalary = data.BasicSalary,
                PositionId = positionId,
                DepartmentId = departmentId,
                DateHired = data.DateHired,
                AcademicAwardId = academicAwardId,
                BenefitId = benefitId,
                EducationalDegreeId = educationalDegreeId,
                EmployeeDeductions = data.EmployeeDeductions,
                EmployeeAdditionalEarnings = data.EmployeeAdditionalEarnings,
                IDNumber = data.IDNumber,
                Email = data.Email,


            };

            _context.Employees.Add(newEmployee);
            _context.SaveChanges();
            return null;
        }

        public bool AddEmployeeDeductions(List<EmployeeDeduction> deductions)
        {
            if (deductions == null || !deductions.Any())
                throw new ArgumentException("No deductions were provided.");

            foreach (var deduction in deductions)
            {
                var employeeId = deduction.EmployeeId;

                if (employeeId == null)
                    throw new KeyNotFoundException("Employee ID is missing.");

                var employee = _context.Employees
                    .FirstOrDefault(e => e.Id == employeeId);

                if (employee == null)
                    throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");

                deduction.EmployeeId = employee.Id;
                deduction.CreatedAt = DateTime.UtcNow;
                deduction.UpdatedAt = DateTime.UtcNow;

                _context.EmployeeDeductions.Add(deduction);
            }

            _context.SaveChanges();
            return true;
        }

        public bool AddEmployeeAllowances(List<Allowance> additionalEarnings)
        {
            if (additionalEarnings == null || !additionalEarnings.Any())
            {
                throw new ArgumentException("No additional earnings were provided.");
            }

            foreach (var additionalEarning in additionalEarnings)
            {
                var employeeId = additionalEarning.EmployeeId;

                if (employeeId == null)
                {
                    throw new KeyNotFoundException("Employee ID is missing.");
                }

                var employee = _context.Employees.FirstOrDefault(e => e.Id == employeeId);

                if (employee == null)
                {
                    throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");
                }

                // Assign timestamps for the record
                additionalEarning.CreatedAt = DateTime.UtcNow;
                additionalEarning.UpdatedAt = DateTime.UtcNow;

                // Add the allowance to the database context
                _context.Allowances.Add(additionalEarning);
            }

            // Save changes to the database
            _context.SaveChanges();
            return true;
        }


        public bool DeleteEmployee(int id)
        {
            var employee = _context.Employees
                                   .Include(e => e.Position)
                                   .Include(e => e.Department)
                                   .Include(e => e.EmployeeDeductions)
                                  .Include(p => p.EmployeeAdditionalEarnings)
                                   .Include(e => e.AcademicAward)
                                   .Include(e => e.Benefit)
                                   .Include(e => e.EducationalDegree)
                                   .FirstOrDefault(e => e.Id == id);

            if (employee == null) return false;

            // Remove related AdditionalEarnings
            if (employee.EmployeeAdditionalEarnings != null && employee.EmployeeAdditionalEarnings.Any())
            {
                _context.Set<Allowance>().RemoveRange(employee.EmployeeAdditionalEarnings);
            }

            if (employee.EmployeeDeductions != null && employee.EmployeeDeductions.Any())
            {
                _context.Set<EmployeeDeduction>().RemoveRange(employee.EmployeeDeductions);
            }

            _context.Employees.Remove(employee);
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _context.Employees
                           .AsNoTracking()
                           .Include(e => e.Position)
                           .Include(e => e.Department)
                           .Include(e => e.AcademicAward)
                           .Include(e => e.Benefit)
                           .Include(e => e.EducationalDegree)
                           .Include(e => e.EmployeeDeductions)
                           .Include(e => e.EmployeeAdditionalEarnings) // Include Additional Earnings
                           .Where(e => e.IsActive)
                           .ToList()
                           .Select(e =>
                           {
                               // Calculate Gross Salary using the Employee's method
                               decimal grossSalary = e.CalculateTotalAllowance();

                               // Calculate total deductions
                               decimal totalDeductions = e.EmployeeDeductions?.Sum(d => d.Amount) ?? 0;

                               // Calculate Net Salary
                               decimal netSalary = grossSalary - totalDeductions;

                               // Calculate Per Day, Per Hour, and Per Minute
                               decimal ratePerDay = e.GetRatePerDay();
                               decimal ratePerHour = e.RatePerHour ?? 0;
                               decimal ratePerMinute = e.RatePerMinute ?? 0;

                               // Set the additional properties for Gross, Net Salary, and Per Day, Per Hour, Per Minute
                               e.GrossSalary = grossSalary;
                               e.RatePerDay = ratePerDay;

                               return e;
                           });
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int employeeId)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId);
        }

    }
}
