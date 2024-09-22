using DataLoggerDatabase.Models;
using Microsoft.EntityFrameworkCore;
using PipesWorkerService;


var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;  // Enable detailed errors for debugging
});
builder.Services.AddHostedService<Worker>();
// builder.Services.AddHostedService<WebSocketWorker>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
// builder.Services.AddTransient<WebSocketWorker>();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
var host = builder.Build();


host.Run();