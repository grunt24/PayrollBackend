using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json.Serialization;

namespace PayrollSystemBackend.Core.Entities
{
    public class Attendance
    {
        public int Id { get; set; }
        public string IDNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public TimeSpan Time { get; set; }

    }
}
