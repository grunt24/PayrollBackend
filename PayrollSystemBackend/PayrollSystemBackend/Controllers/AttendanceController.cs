using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollSystem.DataAccessEFCore;
using PayrollSystemBackend.Core.Entities;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.OpenApi.Any;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using PayrollSystemBackend.Core.Dto.Attendance;
using Microsoft.AspNetCore.Authorization;

namespace PayrollSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllAttendances")]
        public async Task<IActionResult> RequestAttendance()
        {
            var attendances = await _context.Attendances.ToListAsync();
            return Ok(attendances);
        }

        [HttpPost("import")] // Upload Excel file
        public async Task<IActionResult> UploadAttendance(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using var stream = file.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var attendances = new List<Attendance>();

            reader.Read(); // Skip header row
            while (reader.Read())
            {
                if (reader.FieldCount < 5) continue; // Ensure correct format

                var idNumber = reader.GetValue(0)?.ToString()?.Trim() ?? string.Empty;
                var name = reader.GetValue(1)?.ToString()?.Trim() ?? string.Empty;
                var date = reader.GetValue(2) is DateTime dt ? dt : DateTime.MinValue;
                var status = reader.GetValue(3)?.ToString()?.Trim() ?? string.Empty;
                var time = reader.GetValue(4) is DateTime timeValue ? timeValue.TimeOfDay : TimeSpan.Zero;

                if (date == DateTime.MinValue || time == TimeSpan.Zero)
                {
                    continue; // Skip invalid rows
                }

                var attendance = new Attendance
                {
                    IDNumber = idNumber,
                    Name = name,
                    Date = date,
                    Status = status,
                    Time = time
                };
                attendances.Add(attendance);
            }

            await _context.Attendances.AddRangeAsync(attendances);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Attendance records uploaded successfully." });
        }

        [HttpPost("AddAttendance")]
        public async Task<IActionResult> AddAttendance([FromBody] AttendanceDto attendanceDto)
        {
            // Fetch the employee by IDNumber
            var employee = await _context.Employees
                .Where(e => e.IDNumber == attendanceDto.IDNumber)
                .Select(e => new { e.IDNumber, e.FullName })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound(new { Message = "Employee not found." });
            }

            // Check if attendance already exists for the employee on the same date & status
            bool attendanceExists = await _context.Attendances.AnyAsync(a =>
                a.IDNumber == attendanceDto.IDNumber &&
                a.Date.Date == attendanceDto.Date.Date &&
                a.Status == attendanceDto.Status
            );

            if (attendanceExists)
            {
                return Conflict(new { Message = "Attendance for this employee already exists on this date with the same status." });
            }

            try
            {
                var attendance = new Attendance
                {
                    IDNumber = attendanceDto.IDNumber,
                    Name = employee.FullName,
                    Date = attendanceDto.Date,
                    Status = attendanceDto.Status,
                    Time = ParseTime(attendanceDto.Time) // Convert time string
                };

                await _context.Attendances.AddAsync(attendance);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Attendance added successfully!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        private TimeSpan ParseTime(string time)
        {
            string[] formats = { "h:mm tt", "H:mm", "hh:mm tt", "h:mm:ss", "hh:mm:ss tt", "H:mm:ss" };

            if (DateTime.TryParseExact(time.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedTime))
            {
                return parsedTime.TimeOfDay;
            }

            throw new ArgumentException($"Invalid time format: {time}. Expected format: 'h:mm tt' (e.g., '7:00 AM') or 'HH:mm:ss' (e.g., '22:00:00').");
        }

        [HttpPut("Update/{id}")] // Update attendance
        public async Task<IActionResult> UpdateAttendance(int id, [FromBody] AttendanceDto updatedAttendanceDto)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound(new { Message = "Attendance record not found." });
            }

            // Fetch the employee by IDNumber to get the name
            var employee = await _context.Employees
                .Where(e => e.IDNumber == updatedAttendanceDto.IDNumber)
                .Select(e => new { e.IDNumber, e.FullName })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound(new { Message = "Employee not found." });
            }

            // Check if another attendance record with the same date & status exists (excluding current)
            bool attendanceExists = await _context.Attendances.AnyAsync(a =>
                a.IDNumber == updatedAttendanceDto.IDNumber &&
                a.Date.Date == updatedAttendanceDto.Date.Date &&
                a.Status == updatedAttendanceDto.Status &&
                a.Id != id // Exclude the current record from the check
            );

            if (attendanceExists)
            {
                return Conflict(new { Message = "Another attendance record for this employee already exists on this date with the same status." });
            }

            try
            {
                attendance.IDNumber = updatedAttendanceDto.IDNumber;
                attendance.Name = employee.FullName; // Automatically assign the employee's name
                attendance.Date = updatedAttendanceDto.Date;
                attendance.Status = updatedAttendanceDto.Status;
                attendance.Time = ParseTime(updatedAttendanceDto.Time); // Convert time string

                await _context.SaveChangesAsync();

                return Ok(new { Message = "Attendance record updated successfully.", Data = attendance });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }


        [HttpDelete("Delete/{id}")] // Delete attendance
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound("Attendance record not found.");
            }

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Attendance record deleted successfully." });
        }
    }
}
