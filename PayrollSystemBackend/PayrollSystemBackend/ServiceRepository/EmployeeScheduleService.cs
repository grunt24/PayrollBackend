using Microsoft.EntityFrameworkCore;
using PayrollSystem.DataAccessEFCore;
using PayrollSystemBackend.Core.Dto.Schedule;
using PayrollSystemBackend.Core.Entities;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PayrollSystemBackend.ServiceRepository
{
    public class EmployeeScheduleService : IEmployeeScheduleService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeScheduleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddEmployeeSchedule(EmployeeScheduleRequestDto request)
        {
            var employee = await _context.Employees.FindAsync(request.EmployeeId);
            if (employee == null)
            {
                throw new Exception("Employee not found.");
            }

            var existingSchedules = await _context.EmployeeSchedules
                .Where(s => s.EmployeeId == request.EmployeeId)
                .ToListAsync();

            var newSchedules = new List<EmployeeSchedule>();

            foreach (var scheduleDto in request.Schedules)
            {
                var dayOfWeek = (DayOfTheWeek)scheduleDto.DayOfTheWeek;

                // Check if a schedule already exists for this day
                bool isDuplicate = existingSchedules.Any(s => s.DayOfTheWeek == dayOfWeek);
                if (isDuplicate)
                {
                    continue; // Skip duplicate schedules
                }

                // Convert shift start and end to TimeSpan
                TimeSpan shiftStart = scheduleDto.GetShiftStartTime();
                TimeSpan shiftEnd = scheduleDto.GetShiftEndTime();
                TimeSpan allowedOvertime = scheduleDto.GetAllowedOvertime();

                newSchedules.Add(new EmployeeSchedule
                {
                    DayOfTheWeek = dayOfWeek,
                    ShiftStart = shiftStart,
                    ShiftEnd = shiftEnd,
                    AllowedOvertime = allowedOvertime,
                    EmployeeId = request.EmployeeId
                });
            }

            if (newSchedules.Any())
            {
                await _context.EmployeeSchedules.AddRangeAsync(newSchedules);
                await _context.SaveChangesAsync();
                return true;
            }

            return false; // No new schedules added
        }

        public IEnumerable<GetAllEmployeeSchedule> GetEmployeeSchedules()
        {
            return _context.EmployeeSchedules
                .Include(es => es.Employee) 
                .ThenInclude(e => e.Department) 
                .Include(es => es.Employee.Position)
                .AsEnumerable()
                .Select(es => new GetAllEmployeeSchedule
                {
                    EmployeeId = es.EmployeeId,
                    EmployeeName = es.Employee.FullName,
                    Department = es.Employee.Department?.DepartmentName, 
                    Position = es.Employee.Position?.PositionName, 
                    DayOfTheWeek = es.DayOfTheWeek.ToString(),
                    ShiftStart = es.ShiftStart, 
                    ShiftEnd = es.ShiftEnd,     
                    AllowedOvertime = es.AllowedOvertime
                })
                .ToList();
        }

        // Convert TimeSpan to AM/PM format (static method)
        private static string TimeSpanToAMPM(TimeSpan time)
        {
            return DateTime.Today.Add(time).ToString("hh:mm tt", CultureInfo.InvariantCulture); // Ensures AM/PM format
        }

        public async Task<bool> DeleteAllSchedulesAsync(int id)
        {
            var employeeSchedules = await _context.EmployeeSchedules
                .Where(s => s.EmployeeId == id)
                .ToListAsync();

            if (employeeSchedules == null || !employeeSchedules.Any())
            {
                return false; // No schedules found
            }

            _context.EmployeeSchedules.RemoveRange(employeeSchedules);
            await _context.SaveChangesAsync();
            return true;
        }

public async Task<bool> UpdateEmployeeSchedule(EmployeeScheduleRequestDto request)
{
    var employee = await _context.Employees.FindAsync(request.EmployeeId);
    if (employee == null)
    {
        throw new Exception("Employee not found.");
    }

    foreach (var scheduleDto in request.Schedules)
    {
        var dayOfWeek = (DayOfTheWeek)scheduleDto.DayOfTheWeek;
        var shiftStart = scheduleDto.GetShiftStartTime();
        var shiftEnd = scheduleDto.GetShiftEndTime();
        var allowedOvertime = scheduleDto.GetAllowedOvertime();

        var existingSchedule = await _context.EmployeeSchedules
            .FirstOrDefaultAsync(s => s.EmployeeId == request.EmployeeId && s.DayOfTheWeek == dayOfWeek);

        if (existingSchedule != null)
        {
            // ✅ Update existing schedule
            existingSchedule.ShiftStart = shiftStart;
            existingSchedule.ShiftEnd = shiftEnd;
            existingSchedule.AllowedOvertime = allowedOvertime;
        }
        else
        {
            // ➕ Add new schedule if it doesn’t exist
            _context.EmployeeSchedules.Add(new EmployeeSchedule
            {
                EmployeeId = request.EmployeeId,
                DayOfTheWeek = dayOfWeek,
                ShiftStart = shiftStart,
                ShiftEnd = shiftEnd,
                AllowedOvertime = allowedOvertime
            });
        }
    }

    await _context.SaveChangesAsync();
    return true;
}


    }
}
