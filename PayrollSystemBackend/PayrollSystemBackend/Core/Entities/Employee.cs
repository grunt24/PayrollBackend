using payroll_system.Core.Entities;
using PayrollSystem.Domain.Entities;
using PayrollSystemBackend.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Employee : BaseEntity<int>
{
    public string? IDNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleInitial { get; set; }

    [RegularExpression(@"^(\+63|0)9[0-9]{9}$", ErrorMessage = "Invalid Philippine phone number.")]
    public string? ContactNumber { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }

    // FK Basic Salary
    public decimal? BasicSalary { get; set; }

    // FK Position
    public int? PositionId { get; set; }
    [ForeignKey("PositionId")]
    public Position? Position { get; set; }

    // FK Department
    public int? DepartmentId { get; set; }
    [ForeignKey("DepartmentId")]
    public Department? Department { get; set; }

    // FK Academic Award
    public int? AcademicAwardId { get; set; }
    [ForeignKey("AcademicAwardId")]
    public AcademicAward? AcademicAward { get; set; }

    // FK Benefit
    public int? BenefitId { get; set; }
    [ForeignKey("BenefitId")]
    public Benefit? Benefit { get; set; }

    // FK Educational Degree
    public int? EducationalDegreeId { get; set; }
    [ForeignKey("EducationalDegreeId")]
    public EducationalDegree? EducationalDegree { get; set; }

    public ICollection<EmployeeDeduction>? EmployeeDeductions { get; set; }
    public ICollection<Allowance>? EmployeeAdditionalEarnings { get; set; }
    [JsonIgnore]
    public ICollection<EmployeeSchedule>? EmployeeSchedule { get; set; }

    public DateTime? DateHired { get; set; }
    public decimal? GrossSalary { get; set; }
    [NotMapped]
    public decimal? RatePerDay { get; set; }
    [NotMapped]
    public decimal? RatePerHour => RatePerDay / 8;

    [NotMapped]
    public decimal? RatePerMinute => RatePerHour / 60;

    public string FullName => $"{FirstName} {MiddleInitial} {LastName}".Trim();

    public int TotalAbsentDays { get; set; }

    // Calculate Rate Per Day based on Position's Working Days (using BasicSalary + TotalAllowance/ (WorkingDaysPerYear / 12))
    public decimal GetRatePerDay()
    {
        decimal grossSalary = CalculateTotalAllowance();

        if (Position != null && Position.WorkingDaysPerYear.HasValue && Position.WorkingDaysPerYear > 0)
        {
            return (grossSalary) / (Position.WorkingDaysPerYear.Value);
        }
        else if (grossSalary > 0)
        {
            return grossSalary / 30.41666667M; 
        }
        else
        {
            return 0; 
        }
    }

    public decimal CalculateTotalAllowance()
    {
        decimal academicAward = AcademicAward?.AwardAmount ?? 0;
        decimal benefit = Benefit?.BenefitAmount ?? 0;
        decimal educationalDegree = EducationalDegree?.AchievementAmount ?? 0;
        decimal totalAdditionalEarnings = EmployeeAdditionalEarnings?.Sum(e => e.Amount) ?? 0;

        decimal totalAllowance = academicAward + benefit + educationalDegree + totalAdditionalEarnings;

        GrossSalary = (BasicSalary ?? 0) + totalAllowance;
        return GrossSalary.Value;
    }

    public decimal CalculateEmployeeDeduction()
    {
        decimal totalDeductions = EmployeeDeductions?.Where(ed=>ed.IsActive).Sum(ed => ed.Amount) ?? 0;
        return totalDeductions;
    }

}
