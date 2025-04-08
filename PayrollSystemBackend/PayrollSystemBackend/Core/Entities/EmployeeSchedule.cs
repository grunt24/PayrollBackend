using PayrollSystem.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PayrollSystemBackend.Core.Entities
{
    public class EmployeeSchedule : BaseEntity<int>
    {
        public DayOfTheWeek DayOfTheWeek { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan ShiftStart { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan ShiftEnd { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan AllowedOvertime { get; set; }

        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }

    public enum DayOfTheWeek
    {
        Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday
    }
}
