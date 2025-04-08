namespace PayrollSystemBackend.Core.Dto.Schedule
{
    public class EmployeeScheduleRequestDto
    {
        public int EmployeeId { get; set; }
        public List<EmployeeScheduleDto>? Schedules { get; set; }
    }
}
