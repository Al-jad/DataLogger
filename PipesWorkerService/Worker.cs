using DataLoggerDatabase.Models;
using DataLoggerDatabase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace PipesWorkerService;

public class Worker(ILogger<Worker> logger, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly AppSettings? _appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json"));
    private readonly ILogger<Worker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker is starting");
        try
        {
            if (_appSettings is null) throw new ArgumentNullException(nameof(_appSettings));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
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

                        foreach (var data in pipesData)
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
                        var changesCount = await context.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Added {count} Pipe Data at {time}", changesCount, DateTimeOffset.Now);

                        if (changesCount > 0 && _appSettings.StaticStationsUploadedFile != null)
                        {
                            File.Move(_appSettings.StaticStationsFile,
                                _appSettings.StaticStationsUploadedFile, true);
                        }
                    }

                    if (File.Exists(_appSettings.StaticNorthTankFile))
                    {
                        var pipesData = TankPipeDataMap.ParseCsvFile(_appSettings.StaticNorthTankFile);
                        foreach (var item in pipesData)
                        {
                            item.StationId = 6;
                        }
                        context.PipesData.AddRange(pipesData);
                        var changesCount = await context.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Added {count} Tank Data at {time}", changesCount, DateTimeOffset.Now);

                        if (changesCount > 0 && _appSettings.StaticNorthTankUploadedFile != null)
                            File.Move(_appSettings.StaticNorthTankFile, _appSettings.StaticNorthTankUploadedFile, true);
                    }
                    _logger.LogInformation("starting to log data in {delay}ms", _appSettings.Delay);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }
                await Task.Delay(_appSettings.Delay, stoppingToken);
            }
        }
        catch (Exception) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Sender is stopping");
        }
        catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Sender had error");
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
    public string? StaticNorthTankFile { get; set; }
    public string? StaticNorthTankUploadedFile { get; set; }
    public float? SensorDistance { get; set; }
}