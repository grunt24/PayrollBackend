using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using payroll_system.Core.Entities;
using PayrollSystem.DataAccessEFCore;
using PayrollSystem.Domain.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;

namespace payroll_system.Core.Services
{
    public class BenefitService : IBenefitService
    {
        private readonly ApplicationDbContext _context;

        public BenefitService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddBenefit(Benefit data)
        {
            _context.Benefits.Add(data);
            _context.SaveChanges();
            return true;
        }

        public void DeleteBenefit(int id)
        {
            var benefit = _context.Benefits.Find(id);
            if (benefit == null)
            {
                return;
            }

            _context.Benefits.Remove(benefit);
            _context.SaveChanges();
        }

        public IEnumerable<Benefit> GetAll()
        {
            return _context.Benefits.AsNoTracking().ToList();
        }

        public Benefit GetById(int id)
        {
            return _context.Benefits.AsNoTracking().FirstOrDefault(b => b.Id == id) ;
        }

        public void UpdateBenefit(Benefit benefit, int id)
        {
            var existingBenefit = _context.Benefits.Find(id);
            if (existingBenefit == null)
            {
                return;
            }

            existingBenefit.BenefitName = benefit.BenefitName;
            existingBenefit.BenefitDescription = benefit.BenefitDescription;
            existingBenefit.BenefitAmount = benefit.BenefitAmount;
            _context.SaveChanges();
        }
    }
}
