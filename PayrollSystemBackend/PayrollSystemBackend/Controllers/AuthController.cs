using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollSystemBackend.Core.Dto.AuthDto;
using PayrollSystemBackend.Core.Dto.ChangePassword;
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

        //POST: api/admin/seed
       [HttpPost("seed")]
        public async Task<IActionResult> Seed()
        {
            await _authService.SeedAdminUser();
            return Ok("Admin user seeded successfully.");
        }

        //[HttpGet("get-users")]
        //public async Task<IActionResult> GetUsers()
        //{
        //    var result = await _signInManager.UserManager.Users.ToListAsync();
        //    return Ok(result);
        //}

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

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var (success, errors) = await _authService.ChangePasswordAsync(changePasswordDto, User);
            if (!success)
                return BadRequest(new { Errors = errors });

            return Ok(new { Message = "Password changed successfully." });
        }

    }
}
