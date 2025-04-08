using PayrollSystem.Domain.Entities;

namespace payroll_system.Core.Entities
{
    public class EducationalDegree : BaseEntity<int>
    {
        //Masteral, Doctoral
        public string? AchievementName { get; set; }
        public decimal? AchievementAmount { get; set; } 
    }
}
