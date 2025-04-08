using PayrollSystemBackend.Core.Entities.Contributions;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface ISSSContributionService
    {
        Task<bool> ImportFromExcelAsync(IFormFile file);
        Task<IEnumerable<SSSContribution>> GetAllAsync();
        Task<SSSContribution?> GetBySalaryAsync(decimal salary);
        Task<bool> DeleteAllAsync();
    }
}
