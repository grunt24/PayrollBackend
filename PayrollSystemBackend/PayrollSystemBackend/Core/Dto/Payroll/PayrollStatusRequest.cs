using PayrollSystem.Domain.Entities;

namespace PayrollSystemBackend.Core.Dto.Payroll
{
    public class PayrollStatusRequest
    {
        public int? PayrollId { get; set; }
        public int? PayrollStatusId { get; set; }
    }
}
