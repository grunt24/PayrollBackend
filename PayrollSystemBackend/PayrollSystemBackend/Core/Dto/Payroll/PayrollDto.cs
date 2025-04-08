using Microsoft.Identity.Client;
using PayrollSystemBackend.Core.Dto.Calendar;
using PayrollSystemBackend.Core.Dto.Payroll.EmployeeAllowancesDto;
using PayrollSystemBackend.Core.Entities;

public class PayrollDto
{
    public string? IdNumber { get; set; }
    public int Id { get; set; }
    public string? fullName { get; set; }
    public string? PayrollStartDate { get; set; }
    public string? PayrollEndDate { get; set; }
    public int TotalWorkingDays { get; set; }
    public int TotalLatesMinutes { get; set; }
    public int TotalUnderTimeMinutes { get; set; }
    public int TotalAbsentDays { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }
    public decimal BasicSalary { get; set; }

    // Employee Details
    public string? PositionName { get; set; }
    public string? DepartmentName { get; set; }
    public string? AcademicAwardName { get; set; }
    public decimal? AcademicAwardAmount { get; set; }
    public string? BenefitName { get; set; }
    public decimal? BenefitAmount { get; set; }
    public string? EducationalDegreeName { get; set; }
    public decimal? EducationalDegreeAmount { get; set; }

    public string? DateHired { get; set; }

    // Salary Breakdown
    public decimal RatePerDay { get; set; }
    public decimal RatePerHour { get; set; }
    public decimal RatePerMinute { get; set; }

    public decimal TotalAbsentDeduction { get; set; }
    public decimal TotalLatesDeduction { get; set; }
    public decimal TotalUnderTimeDeduction { get; set; }

    public decimal TotalGrossSalary { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? PayrollStatus { get; set; }
    public string? AbsentDates { get; set; }
    public string? HolidayDates { get; set; }
    public List<string?> HolidayNames { get; set; } = [];


    public int HolidayCount { get; set; }
    public decimal TotalHolidayPay { get; set; }
    public decimal NightDifferentialPay { get; set; }
    public decimal TotalNightDifferentialMinutes { get; set; }

    public decimal SssEmployeeShare { get; set; }
    public decimal SssEmployerShare { get; set; }

    public decimal PhilHealthEmployeeShare { get; set; }
    public decimal PhilHealthEmployerShare { get; set; }

    public decimal PagibigEmployeeShare { get; set; }
    public decimal PagibigEmployerShare { get; set; }

    public decimal TotalEmployeeContributions { get; set; } 
    public decimal TotalEmployerContributions { get; set; }
    public decimal TotalContribution { get; set; }

    public string? LeaveDates { get; set; }



    public EmployeeAllowances? EmployeeAllowances { get; set; }

    public List<HolidayDetailDto> HolidayDetails { get; set; } = [];
    public List<EmployeeDeduction> EmployeeDeductions { get; set; } = [];
}
