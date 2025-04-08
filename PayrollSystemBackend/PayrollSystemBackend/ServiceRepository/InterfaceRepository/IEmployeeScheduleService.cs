using PayrollSystem.Domain.Entities;
using PayrollSystemBackend.Core.Dto.Schedule;
using PayrollSystemBackend.Core.Entities;
using System.Threading.Tasks;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface IEmployeeScheduleService
    {
        Task<bool> AddEmployeeSchedule(EmployeeScheduleRequestDto scheduleDto);
        IEnumerable<GetAllEmployeeSchedule> GetEmployeeSchedules();
        Task<bool> DeleteAllSchedulesAsync(int id);
        Task<bool> UpdateEmployeeSchedule(EmployeeScheduleRequestDto request);

    }
}
