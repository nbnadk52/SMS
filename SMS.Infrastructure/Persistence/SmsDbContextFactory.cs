using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SMS.Infrastructure.Persistence;

/// <summary>
/// Design-time factory used by "dotnet ef". The runtime container registers SmsDbContext through
/// Autofac's EFModule rather than AddDbContext, so the EF tooling has no service provider to resolve
/// the context from and needs this factory instead.
/// </summary>
public class SmsDbContextFactory : IDesignTimeDbContextFactory<SmsDbContext>
{
    public SmsDbContext CreateDbContext(string[] args)
    {
        var apiProjectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "SMS.Api"));
        var basePath = Directory.Exists(apiProjectPath) ? apiProjectPath : Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("SmsDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string 'SmsDatabase' was not found. Looked for appsettings files in '{basePath}'.");
        }

        var options = new DbContextOptionsBuilder<SmsDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new SmsDbContext(options);
    }
}
