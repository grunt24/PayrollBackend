//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using payroll_system.Core.Entities;
//using PayrollSystem.DataAccessEFCore;
//using PayrollSystemBackend.InterfaceRepository;

//namespace payroll_system.Core.Services
//{
//    public class BasicSalaryService : IBasicSalaryService
//    {
//        private readonly ApplicationDbContext _context;

//        public BasicSalaryService(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public bool AddBasicSalary(decimal minimumWage)
//        {
//            if (minimumWage <= 0)
//                throw new ArgumentException("Minimum wage must be greater than zero.", nameof(minimumWage));

//            var basicSalary = new BasicSalary { MinimumWage = minimumWage };
//            basicSalary.CalculateBasicSalary();

//            _context.BasicSalary.Add(basicSalary);
//            _context.SaveChanges();
//            return true;
//        }

//        public void DeleteBasicSalary(int id)
//        {
//            var salary = _context.BasicSalary.Find(id);
//            if (salary == null)
//            {
//                return;
//            }

//            _context.BasicSalary.Remove(salary);
//            _context.SaveChanges();
//        }

//        public IEnumerable<BasicSalary> GetAll()
//        {
//            return _context.BasicSalary.AsNoTracking().ToList();
//        }

//        public BasicSalary GetById(int id)
//        {
//            return _context.BasicSalary.AsNoTracking().FirstOrDefault(s => s.Id == id);
//        }

//        public void UpdateBasicSalary(decimal minimumWage, int id)
//        {
//            if (minimumWage <= 0)
//                throw new ArgumentException("Minimum wage must be greater than zero.", nameof(minimumWage));

//            var existingSalary = _context.BasicSalary.Find(id);
//            if (existingSalary == null)
//            {
//                throw new InvalidOperationException("Basic salary not found.");
//            }

//            existingSalary.MinimumWage = minimumWage;
//            existingSalary.CalculateBasicSalary();

//            _context.SaveChanges();
//        }
//    }
//}
