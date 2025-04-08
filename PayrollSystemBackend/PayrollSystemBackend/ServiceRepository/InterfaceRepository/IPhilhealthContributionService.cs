namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface IPhilhealthContributionService
    {
        Task<bool> CreateAsync(PhilHealthContribution philHealthContribution);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<PhilHealthContribution>> GetAllAsync();
        Task<object> GetContributionWithUserSalary(decimal salary);
        decimal ComputePremium(decimal salary);
    }
}
