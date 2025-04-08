using payroll_system.Core.Entities;
using PayrollSystem.Domain.Entities;
using PayrollSystemBackend.Core.Dto.Degree;
using System.Collections.Generic;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface IEducationalDegreeService
    {
        bool AddEducationalDegree(EducationalDegree educationalDegree);
        void DeleteEducationalDegree(int id);
        void UpdateEducationalDegree(EducationalDegree educationalDegree, int id);
        IEnumerable<EducationalDegreeWithCountDto> GetAll();
        EducationalDegree GetById(int id);
        IDictionary<string, int> GetEmployeeCountPerEducationalDegree();
    }
}
