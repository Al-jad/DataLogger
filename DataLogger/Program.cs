using System.Text.Json.Serialization;
using DataLogger;
using DataLoggerDatabase.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PipesWorkerService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();



builder.Services.AddHostedService<WebSocketWorker>();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;  // Enable detailed errors for debugging
});
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
var app = builder.Build();


app.UseRouting();

app.UseCors("CorsPolicy");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
  
}



// Map SignalR hub
app.MapHub<DataHub>("/datahub");

app.MapControllers();



app.Run();






