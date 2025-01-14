using DataLoggerDatabase.Helpers;
using DataLoggerDatabase.Models;
using DataLoggerDatabase;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using DataLogger.DTOs;
namespace DataLogger;

public class WebSocketWorker : BackgroundService
{
    private readonly ILogger<WebSocketWorker> _logger;
    private readonly IHubContext<DataHub, IDataHub> _dataHub;
    private readonly IServiceProvider _serviceProvider;

    public WebSocketWorker(ILogger<WebSocketWorker> logger, IHubContext<DataHub, IDataHub> dataHub, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _dataHub = dataHub;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var latestRecords = await context.PipesData
                .Include(x => x.Station)
                .GroupBy(x => x.StationId)
                .Select(g => g.OrderByDescending(x => x.TimeStamp).First())
                .ToListAsync(stoppingToken);

            var result = new List<PipesToReturnDto>();

            foreach (var record in latestRecords)
            {
                var lastHour = record.TimeStamp.AddHours(-1);
                var lastDay = record.TimeStamp.AddDays(-1);

                var hourlyDischarge = await context.PipesData
                    .Where(x => x.StationId == record.StationId
                        && x.TimeStamp >= lastHour
                        && x.TimeStamp <= record.TimeStamp)
                    .SumAsync(x => x.Discharge ?? 0, stoppingToken);

                var dailyDischarge = await context.PipesData
                    .Where(x => x.StationId == record.StationId
                        && x.TimeStamp >= lastDay
                        && x.TimeStamp <= record.TimeStamp)
                    .SumAsync(x => x.Discharge ?? 0, stoppingToken);

                result.Add(new PipesToReturnDto
                {
                    TimeStamp = record.TimeStamp,
                    StationId = record.StationId,
                    Station = record.Station,
                    Record = record.Record,
                    DischargeInMinute = record.Discharge,
                    DischargeInHour = hourlyDischarge,
                    DischargeInDay = dailyDischarge,
                    Pressure = record.Pressure,
                    WaterLevel = record.Station.TankHeight - record.WaterLevel,
                    CurrentVolume = (record.Station.TankHeight - record.WaterLevel) * record.Station.BaseArea,
                    WaterQuality = record.WaterQuality
                });
            }

            await _dataHub.Clients.All.ReceiveStationData(result);
            
            var latestStationStatus = await context.StationStatus
                .Include(x => x.Station)
                .GroupBy(x => x.StationId)
                .Select(g => g.OrderByDescending(x => x.TimeStamp).First())
                .ToListAsync(stoppingToken);
            
            await _dataHub.Clients.All.ReceiveStationStatus(latestStationStatus);
            await Task.Delay(60000, stoppingToken);

        }
    }
}


public class DataHub : Hub<IDataHub>
{
    private readonly ILogger<DataHub> _logger;

    public DataHub(ILogger<DataHub> logger)
    {
        _logger = logger;
    }
    public async Task SendMessage(string message)
    {
        _logger.LogInformation("Sending message: {Message}", message);
        await Clients.All.SendMessage(message);
    }
    public async Task ReceiveStationData(List<PipesToReturnDto> data)
    {
        //_logger.LogInformation("Sending data for station: {StationId}", data.StationId);
        await Clients.All.ReceiveStationData(data);
    }
    public async Task ReceiveStationStatus(List<StationStatus> data)
    {
        //_logger.LogInformation("Sending data for station: {StationId}", data.StationId);
        await Clients.All.ReceiveStationStatus(data);
    }

}
public interface IDataHub
{
    Task SendMessage(string message);
    Task ReceiveStationData(List<PipesToReturnDto> data);
    Task ReceiveStationStatus(List<StationStatus> data);
}