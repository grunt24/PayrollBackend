using Microsoft.AspNetCore.Identity;
using PayrollSystemBackend.Core.Dto.AuthDto;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface IAuthService
    {
        Task SeedAdminUser();
        Task<LoginResponse?> LoginAsync(LoginDto loginDto);
    }
}
