namespace PayrollSystemBackend.Core.Dto.OrganizationStatisticsDTO
{
    public class OrganizationStatisticsDTO
    {

        public int? TotalDepartments { get; set; }
        public int? TotalEmployees { get; set; }
        public int? TotalPositions { get; set; }
        public int? TotalPayrolls { get; set; }
        public int? TotalEducationalDegrees { get; set; }
    }
    public class DepartmentWithCountDto
    {
        public string DepartmentName { get; set; }
        public int EmployeeCount { get; set; }
    }

    public class PositionWithCountDto
    {
        public string PositionName { get; set; }
        public int EmployeeCount { get; set; }
    }

}
