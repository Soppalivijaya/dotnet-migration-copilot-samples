using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ContosoUniversity.Data
{
    /// <summary>
    /// SchoolContextFactory - Design-time factory for EF Core migrations
    /// This allows EF Core CLI tools to create DbContext instances without a running application
    /// </summary>
    public class SchoolContextFactory : IDesignTimeDbContextFactory<SchoolContext>
    {
        /// <summary>
        /// Creates a SchoolContext instance for design-time operations (migrations)
        /// </summary>
        public SchoolContext CreateDbContext(string[] args)
        {
            const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=ContosoUniversity;Trusted_Connection=true;Encrypt=false";
            var optionsBuilder = new DbContextOptionsBuilder<SchoolContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new SchoolContext(optionsBuilder.Options);
        }
    }
}
