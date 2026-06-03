using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GuitarTrainer.Model
{
    public class DbContext : IdentityDbContext<AppUser>
    {
        public DbContext(DbContextOptions<DbContext> options) : base(options)
        {

        }

        public DbSet<Exercise> Exercises { get; set; }
    }
}
