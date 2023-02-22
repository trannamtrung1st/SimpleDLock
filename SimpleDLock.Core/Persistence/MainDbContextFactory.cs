using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SimpleDLock.Core.Persistence
{
    public class MainDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>
    {
        public MainDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<MainDbContext>()
                .UseSqlServer("Server=localhost,1434;Database=SimpleDLock;Trusted_Connection=False;User Id=sa;Password=z@123456!");

            return new MainDbContext(builder.Options);
        }
    }
}
