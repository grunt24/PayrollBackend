using System.ComponentModel.DataAnnotations;

namespace PayrollSystemBackend.Core.Entities.Contributions
{
    public class PagibigContribution
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal EmployeeContribution { get; set; }

        [Required]
        public decimal EmployerContribution { get; set; }

        public decimal TotalContribution => EmployeeContribution + EmployerContribution;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
