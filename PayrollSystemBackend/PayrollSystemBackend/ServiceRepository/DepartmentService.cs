using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PayrollSystem.DataAccessEFCore;
using PayrollSystem.Domain.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;

namespace payroll_system.Core.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;

        public DepartmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddDepartment(Department department)
        {
            if (department == null)
            {
                return false;
            }

            _context.Departments.Add(department);
            _context.SaveChanges();
            return true;
        }

        public void DeleteDepartment(int id)
        {
            var department = _context.Departments
                                     .Include(d => d.Employees)
                                     .FirstOrDefault(d => d.Id == id);

            if (department == null)
            {
                return;
            }

            if (department.Employees != null && department.Employees.Any())
            {
                foreach (var employee in department.Employees)
                {
                    employee.DepartmentId = null;
                }
            }

            _context.Departments.Remove(department);
            _context.SaveChanges();
        }

        public IEnumerable<Department> GetAll()
        {
            return _context.Departments
                .AsNoTracking()
                .ToList();
        }

        public Department GetById(int id)
        {
            return _context.Departments.FirstOrDefault(d => d.Id == id);
        }

        public void UpdateDepartment(Department department, int id)
        {
            var existingDepartment = _context.Departments.Find(id);
            if (existingDepartment != null)
            {
                existingDepartment.DepartmentName = department.DepartmentName;
                _context.SaveChanges();
            }
        }
    }
}
