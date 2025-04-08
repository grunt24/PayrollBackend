using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;

namespace PayrollSystemBackend.Core.Entities
{
    public class Leave
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public List<DateTime> LeaveDates { get; set; } = new List<DateTime>();
        public bool IsPaid { get; set; } = true;
        //public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    //public enum LeaveStatus
    //{
    //    Pending = 1,
    //    Approved, 
    //    Rejected
    //}
}
