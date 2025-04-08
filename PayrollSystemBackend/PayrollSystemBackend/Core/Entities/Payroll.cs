using payroll_system.Core.Entities;
using PayrollSystemBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PayrollSystem.Domain.Entities
{
    public class Payroll : BaseEntity<int>
    {
        [Required]
        public int EmployeeId { get; set; }  // Foreign key reference

        public Employee? Employee { get; set; }
        public DateTime PayrollStartDate { get; set; }
        public DateTime PayrollEndDate { get; set; }

        public int TotalWorkingDays { get; set; }
        public int TotalLatesMinutes { get; set; }
        public int TotalUnderTimeMinutes { get; set; }
        public int TotalAbsentDays { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public decimal TotalNightDifferentialMinutes { get; set; }
        public decimal NightDifferentialPay { get; set; }
        public string? AbsentDates { get; set; } 
        public string? HolidayDates { get; set; }

        // ✅ **Newly added fields for Contributions**
        public decimal SSSEmployeeShare { get; set; }
        public decimal SSSEmployerShare { get; set; }

        public decimal PhilHealthEmployeeShare { get; set; }
        public decimal PhilHealthEmployerShare { get; set; }

        public decimal PagibigEmployeeShare { get; set; }
        public decimal PagibigEmployerShare { get; set; }

        public decimal TotalEmployeeContributions { get; set; }  // ✅ Fixed duplicate issue
        public decimal TotalEmployerContributions { get; set; }
        public decimal TotalContribution { get; set; }
        public string? LeaveDates { get; set; }

        

        public PayrollStatus? Status { get; set; } = PayrollStatus.Pending;
    }
    public enum PayrollStatus
    {
        Pending = 1, 
        Approved,
        Rejected
    }

}
