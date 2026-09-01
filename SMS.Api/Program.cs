using Autofac;
using Autofac.Extensions.DependencyInjection;
using SMS.Api.Middleware;
using SMS.Application.DependencyInjection;
using SMS.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SmsDatabase")
    ?? throw new InvalidOperationException("Connection string 'SmsDatabase' was not found.");

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    container.RegisterModule(new EFModule(connectionString));
    container.RegisterModule<RepositoryModule>();
    container.RegisterModule<ServiceModule>();
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
