using DataLoggerDatabase.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace PipesWorkerService;

public class Worker(ILogger<Worker> logger, IServiceProvider serviceProvider): BackgroundService
{
    private readonly AppSettings? _appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json"));
    private readonly ILogger<Worker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_appSettings is null) throw new ArgumentNullException(nameof(_appSettings));

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            var query = context.Stations.AsQueryable().Any(
                station => station.SourceAddress == _appSettings.SourceAddress
            );
            if (!query)
            {
                Console.WriteLine("Station not found, adding new station.");
                await context.Stations.AddRangeAsync(_appSettings.Stations, stoppingToken);
                await context.SaveChangesAsync(stoppingToken);
                var station = await context.Stations.FirstAsync(x => x.SourceAddress == _appSettings.SourceAddress, stoppingToken);
                await context.PipesData.Where(x => x.Station == null).ExecuteUpdateAsync(x => x.SetProperty(x => x.StationId, station.Id), stoppingToken);

            }
            
            var stations = await context.Stations.Where(x => x.SourceAddress == _appSettings.SourceAddress).ToListAsync(stoppingToken);

            foreach (var station in stations)
            {
                if (!File.Exists(station.DataFile)) continue;
                var pipesData = PipesDataMap.ParseCsvFile(station.DataFile);
                foreach (var data in pipesData)
                {
                    Console.WriteLine(station.Id);
                    data.StationId = station.Id;
                    Console.WriteLine(data.StationId);
                    data.Station = station;
                }
                
                context.PipesData.AddRange(pipesData);
                var isSaved = await context.SaveChangesAsync(stoppingToken) > 0;
                
                if (isSaved && station.UploadedDataFile != null)
                    File.Move(station.DataFile, station.UploadedDataFile, true);
            }
            // TODO: Tank worker
            await Task.Delay(_appSettings.Delay, stoppingToken);
        }
    }
}

internal class AppSettings
{
    public List<Station> Stations { get; set; } = [];
    public required string SourceAddress { get; init; }
    public int Delay { get; set; }
}
