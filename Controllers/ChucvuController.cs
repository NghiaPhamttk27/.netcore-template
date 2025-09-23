using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using netcoretemplate.Data;
using netcoretemplate.Models;
using Microsoft.EntityFrameworkCore;

namespace netcoretemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChucvuController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChucvuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/chucvu/all
        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Chucvus.ToListAsync();
            return Ok(list);
        }

        // GET: api/chucvu/by-id?id=1
        [HttpGet("by-id")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var chucvu = await _context.Chucvus.FindAsync(id);
            if (chucvu == null) return NotFound();
            return Ok(chucvu);
        }

        // POST: api/chucvu
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Chucvu model)
        {
            model.Active = true; // gán mặc định khi tạo mới

            _context.Chucvus.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }


        // PUT: api/chucvu?id=5
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromQuery] int id, [FromBody] Chucvu model)
        {
            var existing = await _context.Chucvus.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = model.Name;
            existing.Active = model.Active;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // DELETE: api/chucvu?id=5
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            var chucvu = await _context.Chucvus.FindAsync(id);
            if (chucvu == null) return NotFound();

            _context.Chucvus.Remove(chucvu);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
