using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BerkutSignalrBroadcaster.Options;
using Microsoft.Extensions.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(host => {
        host.AddEnvironmentVariables();
    })
    .ConfigureServices((host, services) => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.Configure<BroadcastServiceOptions>(host.Configuration.GetSection(nameof(BroadcastServiceOptions)));
    })
    .Build();

host.Run();
