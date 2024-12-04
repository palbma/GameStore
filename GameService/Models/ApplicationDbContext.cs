using Microsoft.EntityFrameworkCore;

namespace GameService.Models
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameImage> GameImages { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.Entity<Game>()
          .HasOne(g => g.Image)
          .WithOne(i => i.Game)
          .HasForeignKey<Game>(g => g.ImageId)
          .OnDelete(DeleteBehavior.Cascade); 

            base.OnModelCreating(modelBuilder);
        }
    }
}
