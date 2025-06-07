using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServerManagement.Azure;
using ServerManagement.Azure.Configuration;
using ServerManagement.Core.Interfaces;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        // Configure HTTP client
        services.AddHttpClient();
        
        // Configure options
        services.Configure<AzureVmConfiguration>(config =>
        {
            config.SubscriptionId = Environment.GetEnvironmentVariable("AzureVmConfiguration__SubscriptionId") ?? "";
            config.ResourceGroupName = Environment.GetEnvironmentVariable("AzureVmConfiguration__ResourceGroupName") ?? "";
            config.VmName = Environment.GetEnvironmentVariable("AzureVmConfiguration__VmName") ?? "";
        });
        
        services.Configure<GameServerConfiguration>(config =>
        {
            config.Host = Environment.GetEnvironmentVariable("GameServerConfiguration__Host") ?? "";
            if (int.TryParse(Environment.GetEnvironmentVariable("GameServerConfiguration__Port"), out var port))
                config.Port = port;
            if (int.TryParse(Environment.GetEnvironmentVariable("GameServerConfiguration__TelnetPort"), out var telnetPort))
                config.TelnetPort = telnetPort;
            config.AdminPassword = Environment.GetEnvironmentVariable("GameServerConfiguration__AdminPassword") ?? "";
        });
        
        // Register services
        services.AddScoped<IServerManager, AzureServerManager>();
    })
    .Build();

host.Run();