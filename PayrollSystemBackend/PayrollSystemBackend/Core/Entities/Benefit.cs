using PayrollSystem.Domain.Entities;

namespace payroll_system.Core.Entities
{
    public class Benefit : BaseEntity<int>
    {
        public string? BenefitName { get; set; }
        public string? BenefitDescription { get; set; }
        public decimal? BenefitAmount { get; set; }
    }
}
