using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PayrollSystemBackend.Core.Dto.AuthDto;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using System.Threading.Tasks;

namespace PayrollSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(SignInManager<IdentityUser> signInManager, IAuthService authService)
        {
            _signInManager = signInManager;
            _authService = authService;
        }

        // POST: api/admin/seed
        [HttpPost("seed")]
        public async Task<IActionResult> Seed()
        {
            await _authService.SeedAdminUser();
            return Ok("Admin user seeded successfully.");
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginDto model)
        {
            var loginResult = await _authService.LoginAsync(model);

            if (loginResult is null)
            {
                return Unauthorized("Your credentials are invalid. Please try again!");
            }

            return Ok(loginResult);
        }

    }
}
