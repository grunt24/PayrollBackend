using Microsoft.AspNetCore.Identity;
using PayrollSystemBackend.Core.Dto.AuthDto;
using PayrollSystemBackend.Core.Dto.ChangePassword;
using System.Security.Claims;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface IAuthService
    {
        Task SeedAdminUser();
        Task<LoginResponse?> LoginAsync(LoginDto loginDto);
        Task<(bool Success, IEnumerable<string> Errors)> ChangePasswordAsync(ChangePasswordDto changePasswordDto, ClaimsPrincipal userPrincipal);

    }
}
