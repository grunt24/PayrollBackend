using System.ComponentModel.DataAnnotations;

namespace PayrollSystemBackend.Core.Dto.Payroll
{
    public class PayrollAddRequestDto
    {
        [Required]
        public List<string> IdNumbers { get; set; } = [];

        [Required]
        public DateTime PayrollStartDate { get; set; }

        [Required]
        public DateTime PayrollEndDate { get; set; }
    }
}
