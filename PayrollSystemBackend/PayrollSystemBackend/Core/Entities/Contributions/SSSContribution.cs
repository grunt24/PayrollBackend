namespace PayrollSystemBackend.Core.Entities.Contributions
{
    public class SSSContribution
    {
        public int Id { get; set; }
        public decimal MinCompensation { get; set; }
        public decimal MaxCompensation { get; set; }
        //ER
        public decimal EmployerSS { get; set; }
        public decimal EmployerEC { get; set; }
        public decimal EmployerMPF { get; set; } = 0;
        //EE
        public decimal EmployeeSS { get; set; }
        public decimal EmployeeMPF { get; set; } = 0;
        //Total ER
        public decimal TotalEmployerContribution => EmployerSS + EmployerEC + EmployerMPF;
        //Total EE
        public decimal TotalEmployeeContribution => EmployeeSS + EmployeeMPF;
        //ER + EE
        public decimal TotalContribution => TotalEmployerContribution + TotalEmployeeContribution;
        public int Year { get; set; } = DateTime.Now.Year;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
