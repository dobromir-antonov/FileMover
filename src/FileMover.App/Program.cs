using FileMover.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(builder => builder
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build()
    )
    .ConfigureServices((host, services) =>
        services
            .RegisterServices()
            .RegisterLogger(host.Configuration)
    )
    .Build(); 

var console = host.Services.GetRequiredService<IConsoleService>()!;
console.StartUserInputMonitoring();

await host.RunAsync();
