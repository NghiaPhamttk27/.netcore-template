using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using netcoretemplate.Models; // nhớ import namespace chứa model

namespace netcoretemplate.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Thêm DbSet cho bảng Chucvu
        public DbSet<Chucvu> Chucvus { get; set; }
    }
}
