using Autofac;

namespace SMS.Application.DependencyInjection;

public class ServiceModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(ServiceModule).Assembly)
            .Where(t => t.Name.EndsWith("Service", StringComparison.Ordinal) && !t.IsGenericTypeDefinition)
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}
