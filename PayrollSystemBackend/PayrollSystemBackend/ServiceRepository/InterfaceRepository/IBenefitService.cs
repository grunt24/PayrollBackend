using payroll_system.Core.Entities;
using PayrollSystem.Domain.Entities;
using System.Collections.Generic;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface IBenefitService
    {
        bool AddBenefit(Benefit benefit);
        void DeleteBenefit(int id);
        void UpdateBenefit(Benefit benefit, int id);
        IEnumerable<Benefit> GetAll();
        Benefit GetById(int id);
    }
}
