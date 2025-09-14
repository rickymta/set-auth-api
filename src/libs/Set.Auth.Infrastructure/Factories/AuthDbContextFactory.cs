using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Set.Auth.Infrastructure.Data;

namespace Set.Auth.Infrastructure.Factories;

/// <summary>
/// Design-time factory for creating AuthDbContext instances during migrations
/// </summary>
public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    /// <summary>
    /// Creates a new instance of AuthDbContext for design-time operations
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>A configured AuthDbContext instance</returns>
    public AuthDbContext CreateDbContext(string[] args)
    {
        // Load configuration from environment variables or a configuration file as needed
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "Server=192.168.1.127;Database=VolcanionAuth;User=root;Password=123456a@A;";
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)));
        return new AuthDbContext(optionsBuilder.Options);
    }
}
