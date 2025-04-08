using PayrollSystemBackend.Core.Dto.Leave;
using PayrollSystemBackend.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface ILeaveService
    {
        Task<LeaveDto> AddLeaveAsync(LeaveDto leave);
        Task<IEnumerable<LeaveDto>> GetAllLeavesAsync();
        Task<LeaveDto?> GetLeaveByIdAsync(int id);
        Task<IEnumerable<LeaveDto>> GetLeavesByEmployeeIdAsync(int employeeId);
        Task<bool> DeleteLeaveAsync(int id);
        Task<bool> UpdateLeaveAsync(int id, LeaveDto leave);
    }
}
