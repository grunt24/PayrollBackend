using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PayrollSystem.DataAccessEFCore;
using PayrollSystem.Domain.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;

namespace payroll_system.Core.Services
{
    public class AllowanceService : IAllowanceService
    {
        private readonly ApplicationDbContext _context;

        public AllowanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddAdditionalEarning(Allowance data)
        {
            if (data == null || string.IsNullOrEmpty(data.AllowanceName) || data.Amount <= 0)
            {
                return false; // Don't add anything if the input is invalid
            }

            var newEarning = new Allowance
            {
                AllowanceName = data.AllowanceName,
                Amount = data.Amount,
                EmployeeId = data.EmployeeId
            };

            _context.Allowances.Add(newEarning);
            _context.SaveChanges();
            return true;
        }


        public void DeleteAdditionalEarning(int id)
        {
            var earning = _context.Allowances.Find(id);
            if (earning == null)
            {
                return;
            }

            _context.Allowances.Remove(earning);
            _context.SaveChanges();
        }

        public IEnumerable<Allowance> GetAll()
        {
            return _context.Allowances.AsNoTracking().ToList();
        }

        public void UpdateAdditionalEarning(Allowance earning, int id)
        {
            var existingEarning = _context.Allowances.Find(id);
            if (existingEarning == null)
            {
                return;
            }

            existingEarning.AllowanceName = earning.AllowanceName;
            existingEarning.Amount = earning.Amount;
            _context.SaveChanges();
        }
    }
}
