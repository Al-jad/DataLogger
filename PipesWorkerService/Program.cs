using DataLoggerDatabase.Models;
using Microsoft.EntityFrameworkCore;
using PipesWorkerService;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});



var host = builder.Build();
host.Run();