using payroll_system.Core.Entities;
using PayrollSystem.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Position : BaseEntity<int>
{
    public string? PositionName { get; set; }
    [Column(TypeName = "decimal(18,8)")]
    public decimal? WorkingDaysPerYear { get; set; }  // Changed to working days per year
    [JsonIgnore]
    public ICollection<Employee>? Employees { get; set; }
}
