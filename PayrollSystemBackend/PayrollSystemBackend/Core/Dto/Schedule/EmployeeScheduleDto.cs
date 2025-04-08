using System;
using System.Collections.Generic;
using System.Globalization;

public class EmployeeScheduleDto
{
    public int DayOfTheWeek { get; set; }
    public string ShiftStart { get; set; }
    public string ShiftEnd { get; set; }
    public string AllowedOvertime { get; set; }

    public TimeSpan GetShiftStartTime() => ParseTime(ShiftStart);
    public TimeSpan GetShiftEndTime() => ParseTime(ShiftEnd);
    public TimeSpan GetAllowedOvertime() => ParseTime(AllowedOvertime);

    private TimeSpan ParseTime(string time)
    {
        string[] formats = { "h:mm tt", "H:mm", "hh:mm tt", "h:mm:ss", "h:mm:ss tt" };

        if (DateTime.TryParseExact(time.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedTime))
        {
            return parsedTime.TimeOfDay; // Extract only the time part
        }

        throw new ArgumentException($"Invalid time format: {time}. Expected format: 'h:mm tt' (e.g., '7:00 AM').");
    }
}
