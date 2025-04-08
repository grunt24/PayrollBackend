using PayrollSystem.DataAccessEFCore;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;

public class PhilhealthContributionService : IPhilhealthContributionService
{
    private readonly ApplicationDbContext context;

    public PhilhealthContributionService(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<bool> CreateAsync(PhilHealthContribution philHealthContribution)
    {
        if (philHealthContribution == null) return false;

        philHealthContribution.Year = DateTime.Now.Year;

        await context.PhilHealthContributions.AddAsync(philHealthContribution);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<PhilHealthContribution>> GetAllAsync()
    {
        return await context.PhilHealthContributions
            .OrderByDescending(e => e.Year)
            .ToListAsync();
    }

    public decimal ComputePremium(decimal salary)
    {
        var entry = context.PhilHealthContributions
            .FirstOrDefault(e => salary >= e.MinSalary && salary <= e.MaxSalary);

        return entry != null ? salary * entry.PremiumRate : 0m;
    }


    public async Task<object> GetContributionWithUserSalary(decimal salary)
    {
        var entry = await context.PhilHealthContributions
            .FirstOrDefaultAsync(e => salary >= e.MinSalary && salary <= e.MaxSalary);

        if (entry == null)
        {
            return false;
        }

        //sample salary = 10500.55 (EE=262.51 and ER=262.52)

        decimal computedPremium = ComputePremium(salary);

        decimal eeContribution = Math.Floor(computedPremium / 2 * 100) / 100;
        decimal erContribution = computedPremium - eeContribution;


        return new
        {
            entry.Year,
            SalaryRange = entry.SalaryRange,
            entry.PremiumRate,
            MonthlyPremiumRange = $"₱{(entry.MinSalary * entry.PremiumRate):N2} to ₱{(entry.MaxSalary * entry.PremiumRate):N2}",
            ComputedPremium = $"₱{computedPremium:N2}",
            EEContribution = $"₱{eeContribution:N2}", 
            ERContribution = $"₱{erContribution:N2}"
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var philHealth = await context.PhilHealthContributions.FindAsync(id);

        if (philHealth == null)
        {
            return false;
        }

        context.PhilHealthContributions.Remove(philHealth);
        await context.SaveChangesAsync();

        return true;
    }
    
        public async Task<bool> UpdateAsync(int id, PhilHealthContribution updatedContribution)
        {
            var existingContribution = await context.PhilHealthContributions.FindAsync(id);
            if (existingContribution == null) return false;
    
            existingContribution.MinSalary = updatedContribution.MinSalary;
            existingContribution.MaxSalary = updatedContribution.MaxSalary;
            existingContribution.PremiumRate = updatedContribution.PremiumRate;
            existingContribution.MonthlyPremium = updatedContribution.MonthlyPremium;
    
            await context.SaveChangesAsync();
            return true;
        }
}
