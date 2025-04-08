using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using payroll_system.Core.Entities;
using PayrollSystem.DataAccessEFCore;
using PayrollSystem.Domain.Entities;
using PayrollSystemBackend.Core.Dto.Degree;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;

namespace payroll_system.Core.Services
{
    public class EducationalDegreeService : IEducationalDegreeService
    {
        private readonly ApplicationDbContext _context;

        public EducationalDegreeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddEducationalDegree(EducationalDegree data)
        {
            _context.EducationalDegrees.Add(data);
            _context.SaveChanges();
            return true;
        }

        public void DeleteEducationalDegree(int id)
        {
            var degree = _context.EducationalDegrees.Find(id);
            if (degree == null)
            {
                return; // Degree not found
            }

            // Option 1: Dissociate employees from the degree (if employees are referencing it)
            var employees = _context.Employees.Where(e => e.EducationalDegreeId == id).ToList();
            foreach (var employee in employees)
            {
                // Dissociate employee from the educational degree (set the foreign key to null)
                employee.EducationalDegreeId = null;
            }

            // Option 2: Remove the educational degree itself
            _context.EducationalDegrees.Remove(degree);
            _context.SaveChanges();
        }


        public IEnumerable<EducationalDegreeWithCountDto> GetAll()
        {
            return _context.EducationalDegrees
                .AsNoTracking()
                .Select(degree => new EducationalDegreeWithCountDto
                {
                    Id = degree.Id,
                    DegreeName = degree.AchievementName,
                    AchievementAmount = degree.AchievementAmount??0,
                    //CreatedAt = degree.CreatedAt,
                    EmployeeCount = _context.Employees.Count(emp => emp.EducationalDegreeId == degree.Id)
                })
                .ToList();
        }

        public EducationalDegree GetById(int id)
        {
            return _context.EducationalDegrees
                .AsNoTracking()
                .FirstOrDefault(d => d.Id == id);
        }

        public void UpdateEducationalDegree(EducationalDegree degree, int id)
        {
            var existingDegree = _context.EducationalDegrees.Find(id);
            if (existingDegree == null)
            {
                return;
            }

            existingDegree.AchievementName = degree.AchievementName;
            existingDegree.AchievementAmount = degree.AchievementAmount;
            _context.SaveChanges();
        }

        public IDictionary<string, int> GetEmployeeCountPerEducationalDegree()
        {
            var result = _context.Employees
                                 .GroupBy(e => e.EducationalDegree.AchievementName)
                                 .Select(group => new
                                 {
                                     Degree = group.Key,
                                     EmployeeCount = group.Count()
                                 })
                                 .ToDictionary(x => x.Degree, x => x.EmployeeCount);

            return result;
        }

    }


}
