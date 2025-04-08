using PayrollSystemBackend.Core.Entities;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface ICalendarService
    {
        Task<IEnumerable<BCASCalendar>> GetHolidays();
        string AddHoliday(BCASCalendar calendar);
        Task<BCASCalendar> GetById(int id);
        Task<bool> DeleteHoliday(int id);
        Task<bool> UpdateHoliday(BCASCalendar calendar, int id);
    }
}
