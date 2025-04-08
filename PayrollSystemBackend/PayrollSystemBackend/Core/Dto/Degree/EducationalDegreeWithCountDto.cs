namespace PayrollSystemBackend.Core.Dto.Degree
{
    public class EducationalDegreeWithCountDto
    {
        public int Id { get; set; }
        public string DegreeName { get; set; }
        public decimal? AchievementAmount { get; set; }

        //public DateTime CreatedAt { get; set; }
        public int EmployeeCount { get; set; }

    }
}
