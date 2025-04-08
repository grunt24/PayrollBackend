using PayrollSystemBackend.Core.Dto.General;

public class PayrollResult
{
    public bool Success { get; set; }
    public ErrorResponse? Error { get; set; }
}
