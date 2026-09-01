using Autofac;
using Microsoft.EntityFrameworkCore;
using SMS.Infrastructure.Persistence;

namespace SMS.Infrastructure.DependencyInjection;

public class EFModule : Module
{
    private readonly string _connectionString;

    public EFModule(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string is required.", nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(_ => new DbContextOptionsBuilder<SmsDbContext>()
                .UseNpgsql(_connectionString)
                .Options)
            .As<DbContextOptions<SmsDbContext>>()
            .SingleInstance();

        builder.RegisterType<SmsDbContext>()
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}
