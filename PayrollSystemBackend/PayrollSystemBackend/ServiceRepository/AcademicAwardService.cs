using Microsoft.EntityFrameworkCore;
using PayrollSystem.DataAccessEFCore;
using payroll_system.Core.Entities;
using PayrollSystem.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;

namespace payroll_system.Core.Services
{
    public class AcademicAwardService : IAcademicAwardService
    {
        private readonly ApplicationDbContext _context;

        public AcademicAwardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddAcademicAward(AcademicAward data)
        {
            var newAward = new AcademicAward
            {
                AwardName = data.AwardName,
                AwardAmount = data.AwardAmount
            };

            _context.AcademicAwards.Add(newAward);
            _context.SaveChanges();
            return true;
        }

        public void DeleteAcademicAward(int id)
        {
            var award = _context.AcademicAwards.Find(id);
            if (award == null)
            {
                return;
            }

            _context.AcademicAwards.Remove(award);
            _context.SaveChanges();
        }

        public IEnumerable<AcademicAward> GetAll()
        {
            return _context.AcademicAwards.AsNoTracking().ToList();
        }

        public AcademicAward GetById(int id)
        {
            return _context.AcademicAwards.AsNoTracking().FirstOrDefault(a => a.Id == id);
        }

        public void UpdateAcademicAward(AcademicAward award, int id)
        {
            var existingAward = _context.AcademicAwards.Find(id);
            if (existingAward == null)
            {
                return;
            }

            existingAward.AwardName = award.AwardName;
            existingAward.AwardAmount = award.AwardAmount;
            _context.SaveChanges();
        }
    }
}
