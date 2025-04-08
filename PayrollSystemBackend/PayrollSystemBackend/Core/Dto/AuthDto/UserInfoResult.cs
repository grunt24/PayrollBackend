namespace PayrollSystemBackend.Core.Dto.AuthDto
{
    public class UserInfoResult
    {
        public string? NewToken { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public IEnumerable<string>? Role { get; set; }
    }
}
