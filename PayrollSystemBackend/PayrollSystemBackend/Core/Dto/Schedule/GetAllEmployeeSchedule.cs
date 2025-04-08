namespace PayrollSystemBackend.Core.Dto.Schedule
{
    public class GetAllEmployeeSchedule
    {
        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? DayOfTheWeek { get; set; }
        public TimeSpan? ShiftStart { get; set; }   // Keep as TimeSpan
        public TimeSpan? ShiftEnd { get; set; }     // Keep as TimeSpan
        public string? Department { get; set; }
        public string? Position { get; set; }
        public TimeSpan? AllowedOvertime { get; set; }  // Keep as TimeSpan
    }
}
