using FileMover.Infrasturcutre.Services;
using FileMover.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services) =>
        services
            .AddSingleton<ConsoleService>()
            //.AddSingleton<FileMovementQueue>();
            .AddSingleton<IFileMovementQueue, FileMovementQueue>();
    //.AddHostedService<BackgroundFileService>();

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