namespace PayrollSystemBackend.Core.Entities
{
    public class BCASCalendar
    {
        public int Id { get; set; }
        public DateTime HolidayDate { get; set; } = DateTime.UtcNow.Date;
        public string HolidayName { get; set; }  = string.Empty;
        public bool? IsLegal { get; set; } = true;
        public bool IsActive { get; set; }  = true;
    }
}
