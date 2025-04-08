using PayrollSystemBackend.Core.Entities.Contributions;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface IPagibigContributionService
    {
        Task<IEnumerable<PagibigContribution>> GetAllAsync();
        Task<PagibigContribution?> GetByIdAsync(int id);
        Task<bool> CreateAsync(PagibigContribution contribution);
        Task<bool> UpdateAsync(PagibigContribution contribution);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteAllAsync();
    }
}
