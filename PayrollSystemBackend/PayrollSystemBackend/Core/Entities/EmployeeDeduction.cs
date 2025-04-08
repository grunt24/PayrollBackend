using payroll_system.Core.Entities;
using PayrollSystem.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PayrollSystemBackend.Core.Entities
{
    [Table("Employee_Deduction")]
    public class EmployeeDeduction : BaseEntity<int>
    {
        //SSS, Pagibig, BCAS LOAN
        public string?  EmployeeDeductionName { get; set; }
        public decimal? Amount { get; set; }

        // Foreign key to Employee
        public int? EmployeeId { get; set; }
        [JsonIgnore]
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }
    }
}
