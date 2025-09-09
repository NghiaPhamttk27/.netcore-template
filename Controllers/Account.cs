using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace netcoretemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public class RegisterRequest
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        // POST api/account/register
        //{
        //  "userName": "nghiapham",
        //  "email": "nghiapham@example.com",
        //  "password": "Password123!",
        //  "role": "Admin"
        //}
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

            // Tạo user
            var result = await _userManager.CreateAsync(user, model.Password);

            await _userManager.AddToRoleAsync(user, "User");

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"User '{model.UserName}' registered successfully.");
        }

        // deleteh api/account/delete?userName=nghia
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser([FromQuery] string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return BadRequest("UserName is required.");

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound($"User '{userName}' not found.");

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                return Ok($"User '{userName}' deleted successfully.");

            return BadRequest(result.Errors);
        }

        public class UpdateUserRequest
        {
            [Required]
            public string UserName { get; set; }

            public string? NewUserName { get; set; }
            public string? NewEmail { get; set; }
            public string? NewPassword { get; set; }
        }


        //        PUT api/account/update
        //Content-Type: application/json

        //{
        //  "userName": "admin",
        //  "newUserName": "superadmin",
        //  "newEmail": "superadmin@example.com",
        //  "newPassword": "NewPass123!"
        //}

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.UserName))
                return BadRequest("UserName is required.");

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return NotFound($"User '{model.UserName}' not found.");

            // Cập nhật UserName
            if (!string.IsNullOrWhiteSpace(model.NewUserName))
                user.UserName = model.NewUserName;

            // Cập nhật Email
            if (!string.IsNullOrWhiteSpace(model.NewEmail))
                user.Email = model.NewEmail;

            // Update user info
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Đổi password (nếu có nhập)
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!passResult.Succeeded)
                    return BadRequest(passResult.Errors);
            }

            return Ok($"User '{model.UserName}' updated successfully.");
        }


        public class LoginRequest
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("UserName and Password are required.");

            // Lấy user từ UserManager
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return Unauthorized("Invalid username or password.");

            // Kiểm tra password
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
        
            if (!result.Succeeded)
                return Unauthorized("Invalid username or password.");

            return Ok(new
                {
                    user.UserName,
                    user.Email
                }
            );
        }



    }
}
