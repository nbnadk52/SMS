using Autofac;
using SMS.Application.Contracts;
using SMS.Infrastructure.Persistence.Repositories;

namespace SMS.Infrastructure.DependencyInjection;

public class RepositoryModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(Repository<>))
            .As(typeof(IRepository<>))
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(typeof(RepositoryModule).Assembly)
            .Where(t => t.Name.EndsWith("Repository", StringComparison.Ordinal) && !t.IsGenericTypeDefinition)
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}
