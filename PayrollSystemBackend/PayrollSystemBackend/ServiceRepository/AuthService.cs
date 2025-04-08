using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PayrollSystemBackend.Core.Dto.AuthDto;
using PayrollSystemBackend.ServiceRepository.InterfaceRepository;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace payroll_system.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task SeedAdminUser()
        {
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Check if the admin role exists, if not, create it
            if (!await roleManager.RoleExistsAsync("TimeKeeper"))
            {
                await roleManager.CreateAsync(new IdentityRole("TimeKeeper"));
            }

            // Check if the admin user already exists
            var adminUser = await userManager.FindByNameAsync("timekeeper");
            if (adminUser == null)
            {
                // Create the admin user
                adminUser = new IdentityUser
                {
                    UserName = "TimeKeeper",
                    Email = "TimeKeeper@gmail.com",
                    EmailConfirmed = true
                };

                // Create the user with a password
                var result = await userManager.CreateAsync(adminUser, "TimeKeeper123!");
                if (result.Succeeded)
                {
                    // Assign the admin role to the user
                    await userManager.AddToRoleAsync(adminUser, "TimeKeeper");
                }
            }
        }

        public async Task<LoginResponse?> LoginAsync(LoginDto loginDto)
        {
            // Find user by username
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user is null)
                return null;

            // Check password
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordCorrect)
                return null;

            // Generate JWT token
            //var newToken = await GenerateJWTTokenAsync(users);
            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = await GenerateUserInfoObject(user, roles);

            // Return UserInfoResult
            return new LoginResponse()
            {
                UserInfo = userInfo,
            };
        }
        public async Task<UserInfoResult> GenerateUserInfoObject(IdentityUser user, IEnumerable<string> role)
        {
            // Generate the JWT token for the user
            var newToken = await GenerateJWTTokenAsync(user);

            return new UserInfoResult()
            {
                NewToken = newToken,
                UserName = user.UserName,
                Email = user.Email,
                Role = role
            };
        }

        private async Task<string> GenerateJWTTokenAsync(IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var signingCredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256);

            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: signingCredentials
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;
        }



    }
}