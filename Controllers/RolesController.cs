using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace netcoretemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // api/roles
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("Role name is required.");

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (result.Succeeded)
                return Ok($"Role '{roleName}' created.");

            return BadRequest(result.Errors);
        }

        // api/roles?roleName=Admin
        [HttpDelete]
        public async Task<IActionResult> DeleteRole([FromQuery] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("Role name is required.");

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound($"Role '{roleName}' not found.");

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
                return Ok($"Role '{roleName}' deleted.");

            return BadRequest(result.Errors);
        }

        // GET: api/roles
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }
    }
}
