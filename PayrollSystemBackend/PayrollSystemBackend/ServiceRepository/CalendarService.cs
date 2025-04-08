using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PayrollSystem.DataAccessEFCore;
using PayrollSystemBackend.Core.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;

namespace PayrollSystemBackend.ServiceRepository
{
    public class CalendarService(ApplicationDbContext _context) : ICalendarService
    {
        public string AddHoliday(BCASCalendar calendar)
        {
            if (calendar == null) throw new ArgumentNullException(nameof(calendar));

            bool isExist = _context.Calendars.Any(x => x.HolidayDate == calendar.HolidayDate);
            if (isExist)
            {
                return "A holiday with this date already exists.";
            }

            _context.Add(calendar);
            _context.SaveChanges();
            return "Holiday added successfully!";
        }


        public async Task<bool> DeleteHoliday(int id)
        {
            var holiday = await _context.Calendars.FindAsync(id);

            if (holiday == null) return false;

            _context.Calendars.Remove(holiday);
            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<BCASCalendar> GetById(int id)
        {
            var holiday = await _context.Calendars.FindAsync(id);
            if (holiday == null)
            {
                throw new KeyNotFoundException("Holiday not found");
            }
            return (holiday);
        }

        public async Task<IEnumerable<BCASCalendar>> GetHolidays()
        {
            var holidays = await _context.Calendars.Where(c=>c.IsActive).ToListAsync();
            return holidays;
        }

        public async Task<bool> UpdateHoliday(BCASCalendar calendar, int id)
        {
            var existingHoliday = await _context.Calendars.FindAsync(id);

            if (existingHoliday == null) return false;

            existingHoliday.HolidayDate = calendar.HolidayDate;
            existingHoliday.HolidayName = calendar.HolidayName;
            existingHoliday.IsActive = calendar.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
