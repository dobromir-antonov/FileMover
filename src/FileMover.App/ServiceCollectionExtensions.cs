using FileMover.Infrasturcutre.Services;
using FileMover.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services) =>
        services
            .AddSingleton<IConsoleService, ConsoleService>()
            .AddSingleton<IFileMovementQueue, FileMovementQueue>();

    public static IServiceCollection RegisterLogger(this IServiceCollection services, IConfiguration configuration) =>
       services.AddLogging(config =>
       {
           config.ClearProviders();

           var logger = new LoggerConfiguration()
               .ReadFrom.Configuration(configuration)
               .Enrich.FromLogContext()
               .CreateLogger();

           config.AddSerilog(logger);
       });
}