using PayrollSystem.Domain.Entities;

namespace payroll_system.Core.Entities
{
    public class AcademicAward : BaseEntity<int>
    {
        //Latin Honors, Cum-Laude, Zuma, Magna
        public string? AwardName { get; set; }
        public decimal? AwardAmount { get; set; }
    }
}
