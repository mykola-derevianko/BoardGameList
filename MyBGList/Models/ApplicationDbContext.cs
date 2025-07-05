using Microsoft.EntityFrameworkCore;

namespace MyBGList.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BoardGames_Domains>()
                .HasKey(bg => new { bg.BoardGameId, bg.DomainId });

            modelBuilder.Entity<BoardGames_Domains>()
                .HasOne(bg => bg.BoardGame)
                .WithMany(d => d.BoardGames_Domains)
                .HasForeignKey(f => f.BoardGameId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BoardGames_Domains>()
                .HasOne(d => d.Domain)
                .WithMany(d => d.BoardGames_Domains)
                .HasForeignKey(f => f.DomainId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<BoardGames_Mechanics>()
                .HasKey(bg => new { bg.BoardGameId, bg.MechanicId });

            modelBuilder.Entity<BoardGames_Mechanics>()
                .HasOne(bg => bg.BoardGame)
                .WithMany(m => m.BoardGames_Mechanics)
                .HasForeignKey(fk => fk.BoardGame)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BoardGames_Mechanics>()
                .HasOne(m => m.Mechanic)
                .WithMany(bgm => bgm.BoardGames_Mechanics)
                .HasForeignKey(fk => fk.MechanicId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            
        }

        public DbSet<BoardGame> BoardGames { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<Mechanic> Mechanics { get; set; }
        public DbSet<BoardGames_Domains> BoardGames_Domains { get; set; }
        public DbSet<BoardGames_Mechanics> BoardGames_Mechanics { get; set; }
        
    }
}
