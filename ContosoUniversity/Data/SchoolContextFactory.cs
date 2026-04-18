using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Data
{
    /// <summary>
    /// SchoolContextFactory - Legacy factory class
    /// In ASP.NET Core, DbContext instances are created via dependency injection.
    /// The Create method is maintained for backward compatibility only.
    /// </summary>
    public static class SchoolContextFactory
    {
        /// <summary>
        /// Creates a SchoolContext instance with a default connection string.
        /// For production, use dependency injection instead.
        /// </summary>
        public static SchoolContext Create()
        {
            const string connectionString = "Server=DESKTOP-OJ4OBJ7;Database=ContosoUniversity;Trusted_Connection=true;Encrypt=false";
            var optionsBuilder = new DbContextOptionsBuilder<SchoolContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new SchoolContext(optionsBuilder.Options);
        }
    }
}
