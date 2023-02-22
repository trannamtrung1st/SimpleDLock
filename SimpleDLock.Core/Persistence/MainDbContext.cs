using Microsoft.EntityFrameworkCore;
using SimpleDLock.Core.Entities;
using System.Linq;

namespace SimpleDLock.Core.Persistence
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {
        }

        public MainDbContext()
        {
        }

        public virtual DbSet<FieldEntity> Field { get; set; }
        public virtual DbSet<BookingEntity> Booking { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookingEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<FieldEntity>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.HasData(Enumerable.Range(1, 18).Select(i => new FieldEntity($"Field {i:00}")));
            });
        }
    }
}
