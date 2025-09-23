using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace netcoretemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public class RegisterRequest
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Gán mặc định role User
            await _userManager.AddToRoleAsync(user, "User");

            return Ok($"User '{model.UserName}' registered successfully.");
        }

        public class LoginRequest
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            public string Password { get; set; }
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromBody] LoginRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, model.Password)))
            {
                return Unauthorized();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(userRoles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddDays(14);

            var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Issuer"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new Dictionary<string, object>
                {
                    { "access_token", accessToken },
                    { "token_type", "bearer" },
                    { "expires_in", (int)TimeSpan.FromDays(14).TotalSeconds },
                    { "userName", user.UserName },
                    { "userId", user.Id },
                    { "role", userRoles.FirstOrDefault() ?? "User" },
                    { ".issued", DateTime.UtcNow.ToString("r") },
                    { ".expires", expires.ToString("r") }
                });
        }


    }
}
