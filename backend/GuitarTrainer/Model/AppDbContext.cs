using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GuitarTrainer.Model
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
        public DbSet<Sample> Samples { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(entity =>
            {
                entity.Property(user => user.FirstName).HasMaxLength(100);
                entity.Property(user => user.LastName).HasMaxLength(100);
            });
            builder.Entity<Exercise>(entity =>
            {
                entity.Property(e => e.Title).HasMaxLength(100);
                entity.HasIndex(e => e.Title).IsUnique();
            });
            builder.Entity<Sample>(entity =>
            {
                entity.Property(s => s.Path).HasMaxLength(255);
            });
            builder.Entity<AnswerOption>(entity =>
            {
                entity.HasIndex(a => new { a.ExerciseId, a.TitleCode}).IsUnique();
                entity.Property(a => a.TitleCode).HasMaxLength(100);
            });

        }
    }
}
