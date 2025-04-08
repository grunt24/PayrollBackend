using PayrollSystem.Domain.Entities;

namespace PayrollSystemBackend.Core
{
    public class EmployeeResponseDto
    {
        public List<Employee> Employees { get; set; } // List of employees
        public int TotalEmployeeCount { get; set; } // Total employee count
    }
}
