using PayrollSystem.Domain.Entities;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface IAllowanceService
    {
        bool AddAdditionalEarning(Allowance allowance);
        void DeleteAdditionalEarning(int id);
        void UpdateAdditionalEarning(Allowance allowance, int id);
        IEnumerable<Allowance> GetAll();
    }
}
