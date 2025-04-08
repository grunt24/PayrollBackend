using payroll_system.Core.Entities;
using PayrollSystem.Domain.Entities;
using System.Collections.Generic;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface IAcademicAwardService
    {
        bool AddAcademicAward(AcademicAward academicAward);
        void DeleteAcademicAward(int id);
        void UpdateAcademicAward(AcademicAward academicAward, int id);
        IEnumerable<AcademicAward> GetAll();
        AcademicAward GetById(int id);
    }
}
