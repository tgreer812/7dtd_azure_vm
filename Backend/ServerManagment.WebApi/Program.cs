using ServerManagement.Azure;
using ServerManagement.Azure.Configuration;
using ServerManagement.Core.Interfaces;
using ServerManagment.WebApi;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

builder.Services.Configure<AzureVmConfiguration>(builder.Configuration.GetSection("AzureVmConfiguration"));
builder.Services.Configure<GameServerConfiguration>(builder.Configuration.GetSection("GameServerConfiguration"));

builder.Services.AddScoped<IServerManager, AzureServerManager>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapVmEndpoints();
app.MapGameEndpoints();

app.Run();
