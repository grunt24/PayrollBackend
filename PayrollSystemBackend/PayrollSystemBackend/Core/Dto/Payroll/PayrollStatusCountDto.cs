namespace PayrollSystemBackend.Core.Dto.Payroll
{
    public class PayrollStatusCountDto
    {
        public int ApprovedCount { get; set; }
        public int PendingCount { get; set; }
        public int RejectedCount { get; set; }
    }
}
