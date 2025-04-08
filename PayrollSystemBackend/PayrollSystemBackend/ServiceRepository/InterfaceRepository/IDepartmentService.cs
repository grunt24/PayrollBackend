using PayrollSystem.Domain.Entities;
using System.Collections.Generic;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface IDepartmentService
    {
        bool AddDepartment(Department department);
        void DeleteDepartment(int id);
        void UpdateDepartment(Department department, int id);
        IEnumerable<Department> GetAll();
        Department GetById(int id);
    }
}
