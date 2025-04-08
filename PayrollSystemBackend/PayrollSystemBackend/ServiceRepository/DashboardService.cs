using Microsoft.EntityFrameworkCore;
using PayrollSystem.DataAccessEFCore;
using PayrollSystem.Domain.Entities;
using PayrollSystemBackend.Core.Dto.OrganizationStatisticsDTO;

namespace PayrollSystemBackend.ServiceRepository
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }
        public OrganizationStatisticsDTO GetAll()
        {

            return new OrganizationStatisticsDTO
            {
                TotalDepartments = _context.Departments.Count(),
                TotalEmployees = _context.Employees.Count(),
                TotalPositions = _context.Positions.Count(),
                TotalPayrolls = _context.Payrolls.Count(),
                TotalEducationalDegrees = _context.EducationalDegrees.Count()
            };
        }
        // New method to get Department statistics
        public IEnumerable<DepartmentWithCountDto> GetDepartmentsWithEmployeeCount()
        {
            return _context.Departments
                .AsNoTracking()
                .Select(department => new DepartmentWithCountDto
                {
                    DepartmentName = department.DepartmentName,
                    EmployeeCount = _context.Employees.Count(emp => emp.DepartmentId == department.Id)
                })
                .ToList();
        }

        // New method to get Position statistics
        public IEnumerable<PositionWithCountDto> GetPositionsWithEmployeeCount()
        {
            return _context.Positions
                .AsNoTracking()
                .Select(position => new PositionWithCountDto
                {
                    PositionName = position.PositionName,
                    EmployeeCount = _context.Employees.Count(emp => emp.PositionId == position.Id)
                })
                .ToList();
        }

    }
}
