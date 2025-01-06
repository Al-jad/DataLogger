using DataLoggerDatabase.Models;
using DataLoggerDatabase;
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

            if (File.Exists(_appSettings.StaticStationsFile))
            {
                var pipesData = PipesDataMap.ParseCsvFile(_appSettings.StaticStationsFile);
                var staticStationsData = new List<PipesData>();

                foreach(var data in pipesData)
                {
                    var dischargePressureData = new PipesData
                        {
                            StationId = 4,
                            Discharge = data.Discharge,
                            Pressure = data.Pressure,
                            TimeStamp = data.TimeStamp
                        };
                        staticStationsData.Add(dischargePressureData);

                        var discharge2Data = new PipesData
                        {
                            StationId = 5,
                            Discharge = data.Discharge2,
                            TimeStamp = data.TimeStamp
                        };
                        staticStationsData.Add(discharge2Data);
                }
                    context.PipesData.AddRange(staticStationsData);
                    var isSaved = await context.SaveChangesAsync(stoppingToken) > 0;

                    if (isSaved && _appSettings.StaticStationsUploadedFile != null)
                    {
                        File.Move(_appSettings.StaticStationsFile,
                            _appSettings.StaticStationsUploadedFile, true);
                    }

            }
            
            foreach (var station in stations)
            {
                if (station.Id == 4 || station.Id == 5) continue;
                if (File.Exists(station.DataFile))
                {
                    var pipesData = PipesDataMap.ParseCsvFile(station.DataFile);

                    foreach (var data in pipesData)
                    {
                        data.StationId = station.Id;
                        data.Station = station;
                    }

                    context.PipesData.AddRange(pipesData);
                    var isSaved = await context.SaveChangesAsync(stoppingToken) > 0;

                    if (isSaved && station.UploadedDataFile != null)
                    {
                        File.Move(station.DataFile,
                            station.UploadedDataFile, true);
                    }
                };
            }
            // TODO: Tank worker
            _logger.LogInformation("starting to log data in {delay}ms", _appSettings.Delay);
            await Task.Delay(_appSettings.Delay, stoppingToken);
        }
    }
}

internal class AppSettings
{
    public List<Station> Stations { get; set; } = [];
    public required string SourceAddress { get; init; }
    public int Delay { get; set; }
    public string? StaticStationsFile { get; set; }
    public string? StaticStationsUploadedFile { get; set; }
}
