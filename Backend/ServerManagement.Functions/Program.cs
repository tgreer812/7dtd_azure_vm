using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServerManagement.Azure;
using ServerManagement.Azure.Configuration;
using ServerManagement.Core.Interfaces;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, config) =>
    {
        // Add environment variables as a configuration source
        // This allows the app to read settings from:
        // - local.settings.json (when running locally)
        // - Azure Function App Settings (when deployed)
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        // Configure HTTP client
        services.AddHttpClient();
        
        // Bind configuration sections to strongly-typed classes
        // This maps environment variables to properties based on naming convention:
        // - AzureVmConfiguration__SubscriptionId -> AzureVmConfiguration.SubscriptionId
        // - AzureVmConfiguration__ResourceGroupName -> AzureVmConfiguration.ResourceGroupName
        // - AzureVmConfiguration__VmName -> AzureVmConfiguration.VmName
        services.Configure<AzureVmConfiguration>(
            context.Configuration.GetSection("AzureVmConfiguration"));
        
        // Similarly for GameServerConfiguration:
        // - GameServerConfiguration__Host -> GameServerConfiguration.Host
        // - GameServerConfiguration__Port -> GameServerConfiguration.Port (auto-converted to int)
        // - GameServerConfiguration__TelnetPort -> GameServerConfiguration.TelnetPort (auto-converted to int)
        // - GameServerConfiguration__AdminPassword -> GameServerConfiguration.AdminPassword
        services.Configure<GameServerConfiguration>(
            context.Configuration.GetSection("GameServerConfiguration"));
        
        // Register services
        // The AzureServerManager will receive the configuration objects via IOptions<T> injection
        services.AddScoped<IServerManager, AzureServerManager>();
    })
    .Build();

host.Run();