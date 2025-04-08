using Microsoft.EntityFrameworkCore;
using PayrollSystem.DataAccessEFCore;
using PayrollSystemBackend.Core.Entities.Contributions;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;

namespace PayrollSystemBackend.ServiceRepository
{
    public class PagibigContributionService : IPagibigContributionService
    {
        private readonly ApplicationDbContext _context;

        public PagibigContributionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PagibigContribution>> GetAllAsync()
        {
            return await _context.PagibigContributions.ToListAsync();
        }

        public async Task<PagibigContribution?> GetByIdAsync(int id)
        {
            return await _context.PagibigContributions.FindAsync(id);
        }

        public async Task<bool> CreateAsync(PagibigContribution contribution)
        {
            await _context.PagibigContributions.AddAsync(contribution);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(PagibigContribution contribution)
        {
            var existing = await _context.PagibigContributions.FindAsync(contribution.Id);
            if (existing == null) return false;

            existing.EmployeeContribution = contribution.EmployeeContribution;
            existing.EmployerContribution = contribution.EmployerContribution;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.PagibigContributions.FindAsync(id);
            if (existing == null) return false;

            _context.PagibigContributions.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAllAsync()
        {
            var allContributions = await _context.PagibigContributions.ToListAsync();
            if (!allContributions.Any()) return false;

            _context.PagibigContributions.RemoveRange(allContributions);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
