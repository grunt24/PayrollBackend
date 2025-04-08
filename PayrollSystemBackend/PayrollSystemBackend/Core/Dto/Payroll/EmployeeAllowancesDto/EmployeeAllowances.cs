using payroll_system.Core.Entities;
using PayrollSystem.Domain.Entities;

namespace PayrollSystemBackend.Core.Dto.Payroll.EmployeeAllowancesDto
{
    public class EmployeeAllowances
    {
        public List<AcademicAward>? AcademicAwards { get; set; }
        public List<Allowance>? AdditionalEarning { get; set; }
        public List<Benefit>? Benefits { get; set; }
        public List<EducationalDegree>? EducationalDegrees { get; set; }
    }
}
