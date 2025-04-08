using PayrollSystemBackend.Core.Dto.Employee;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PayrollSystemBackend.Core.Dto.Leave
{
    public class LeaveDto
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public List<DateTime> LeaveDates { get; set; } = new List<DateTime>();

        public bool IsPaid { get; set; } = true;

        [Required]
        [MaxLength(255)]
        public string Reason { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
