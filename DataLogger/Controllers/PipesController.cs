using DataLoggerDatabase.Models;
using DataLoggerDatabase;
using Microsoft.AspNetCore.Mvc;
using DataLogger.DTOs;
using Microsoft.EntityFrameworkCore;
using static DataLoggerDatabase.Enums;
using static DataLoggerDatabase.AppDbContext;

namespace DataLogger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PipesController(AppDbContext context) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PipesData>>> GetPipesData()
        {
            return await context.PipesData.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<PipesData>> PostPipesData([FromBody] PipesDataDto pipeDto)
        {
            var pipe = new PipesData
            {
                TimeStamp = DateTime.UtcNow,
                StationId = pipeDto.StationId,
                Record = pipeDto.Record,
                // Station = station,
                Discharge = pipeDto.Discharge,
                TotalVolumePerHour = pipeDto.TotalVolumePerHour,
                TotalVolumePerDay = pipeDto.TotalVolumePerDay,
                Pressure = pipeDto.Pressure,
                CL = pipeDto.CL,
                Turbidity = pipeDto.Turbidity,
                ElectricConductivity = pipeDto.ElectricConductivity
            };

            context.PipesData.Add(pipe);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPipesDataById), new { id = pipe.Id }, pipe);
        }

        [HttpGet("{id}")]
        public ActionResult<PipesData> GetPipesDataById(long id)
        {
            var pipe = context.PipesData.FirstOrDefault(s => s.Id == id);
            if (pipe == null) return NotFound();
            return Ok(pipe);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPipesDataById(long id, [FromBody] PipesDataDto updatedPipesDataDto)
        {
            var pipe = context.PipesData.FirstOrDefault(s => s.Id == id);
            // var station = context.Stations.FirstOrDefault(s => s.Id == updatedPipesDataDto.StationId);

            if (pipe == null) return NotFound();

            pipe.Record = updatedPipesDataDto.Record;
            pipe.StationId = updatedPipesDataDto.StationId;
            pipe.Discharge = updatedPipesDataDto.Discharge;
            // pipe.Station = station;
            pipe.TotalVolumePerHour = updatedPipesDataDto.TotalVolumePerHour;
            pipe.TotalVolumePerDay = updatedPipesDataDto.TotalVolumePerDay;
            pipe.Pressure = updatedPipesDataDto.Pressure;
            pipe.CL = updatedPipesDataDto.CL;
            pipe.Turbidity = updatedPipesDataDto.Turbidity;
            pipe.ElectricConductivity = updatedPipesDataDto.ElectricConductivity;

            context.Entry(pipe).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("daily_discharge")]
        public async Task<IActionResult> get_daily_discharge(DateTime startDate, DateTime endDate)
        {
            var dailyDischarge = await context.PipesData
                .Where(p => p.TimeStamp.Date >= DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc) && p.TimeStamp.Date <= DateTime.SpecifyKind(endDate.Date, DateTimeKind.Utc))
                .GroupBy(p => p.StationId)
                .Select(g => new
                {
                    StationId = g.Key,
                    DailyDischarge = g.Sum(p => p.Discharge),
                    DailyDischarge2 = g.Sum(p => p.Discharge2)
                })
                .Join(
                    context.Stations,
                    pd => pd.StationId,
                    s => s.Id,
                    (pd, s) => new
                    {
                        StationId = s.Id,
                        StationName = s.Name,
                        s.City,
                        pd.DailyDischarge
                    }
                )
                .ToListAsync();

            return Ok(dailyDischarge);
        }

        [HttpGet("hourly")]
        public async Task<IActionResult> GetHourly(long stationId, int skip, int take = 10)
        {
            var query = context.PipesData
                .Where(x => x.StationId == stationId)
                .GroupBy(p => new DateTime(p.TimeStamp.Year, p.TimeStamp.Month, p.TimeStamp.Day, p.TimeStamp.Hour, 0, 0))
                .OrderByDescending(x => x.Key)
                .Select(g => new
                {
                    Date = g.Key,
                    Discharge = g.Sum(p => p.Discharge),
                    Discharge2 = g.Sum(p => p.Discharge2),
                    BatteryVoltage = g.Average(p => p.BatteryVoltage),
                    Temperature = g.Average(p => p.Temperature),
                    Pressure = g.Average(p => p.Pressure),
                    Pressure2 = g.Average(p => p.Pressure2),
                });

            var count = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Date)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return Ok(new { count, data });
        }

        [HttpGet("daily")]
        public async Task<IActionResult> GetDaily(long stationId, int skip, int take = 10)
        {
            var query = context.PipesData
                .Where(x => x.StationId == stationId)
                .GroupBy(p => new DateOnly(p.TimeStamp.Year, p.TimeStamp.Month, p.TimeStamp.Day))
                .OrderByDescending(x => x.Key)
                .Select(g => new
                {
                    Date = g.Key,
                    Discharge = g.Sum(p => p.Discharge),
                    Discharge2 = g.Sum(p => p.Discharge2),
                    BatteryVoltage = g.Average(p => p.BatteryVoltage),
                    Temperature = g.Average(p => p.Temperature),
                    Pressure = g.Average(p => p.Pressure),
                    Pressure2 = g.Average(p => p.Pressure2),
                });

            var count = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Date)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return Ok(new { count, data });
        }

        [HttpGet("realtime")]
        public async Task<IActionResult> GetHourlyRecords(long stationId, ByDuration byDuration, DateTime? fromDate, DateTime? toDate, int skip, int take = 10)
        {
            var baseQuery = context.PipesData
                .Where(x => x.StationId == stationId);

            if (fromDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.TimeStamp >= fromDate.Value.ToUniversalTime());
            }

            if (toDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.TimeStamp <= toDate.Value.ToUniversalTime());
            }

            var query = byDuration switch
            {
                ByDuration.Hour => baseQuery
                    .GroupBy(p => DateTrunc(TruncField.hour, p.TimeStamp))
                    .Select(g => new
                    {
                        Date = g.Key,
                        Discharge = g.Sum(p => p.Discharge),
                        Discharge2 = g.Sum(p => p.Discharge2),
                        BatteryVoltage = g.Average(p => p.BatteryVoltage),
                        Temperature = g.Average(p => p.Temperature),
                        Pressure = g.Average(p => p.Pressure),
                        Pressure2 = g.Average(p => p.Pressure2),
                        WaterLevel = g.Average(p => p.WaterLevel),
                        CurrentVolume = g.Average(p => (p.WaterLevel) * p.Station.BaseArea),
                        WaterQuality = g.Average(p => p.WaterQuality)
                    }),
                ByDuration.Day => baseQuery
                    .GroupBy(p => p.TimeStamp.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Discharge = g.Sum(p => p.Discharge),
                        Discharge2 = g.Sum(p => p.Discharge2),
                        BatteryVoltage = g.Average(p => p.BatteryVoltage),
                        Temperature = g.Average(p => p.Temperature),
                        Pressure = g.Average(p => p.Pressure),
                        Pressure2 = g.Average(p => p.Pressure2),
                        WaterLevel = g.Average(p => p.WaterLevel),
                        CurrentVolume = g.Average(p => p.WaterLevel * p.Station.BaseArea),
                        WaterQuality = g.Average(p => p.WaterQuality)
                    }),
                _ => baseQuery
                    .Select(x => new
                    {
                        Date = x.TimeStamp,
                        x.Discharge,
                        x.Discharge2,
                        x.BatteryVoltage,
                        x.Temperature,
                        x.Pressure,
                        x.Pressure2,
                        x.WaterLevel,
                        CurrentVolume = x.WaterLevel * x.Station.BaseArea,
                        x.WaterQuality
                    })
            };

            int count = byDuration switch
            {
                ByDuration.Hour => await baseQuery.GroupBy(p => DateTrunc(TruncField.hour, p.TimeStamp)).CountAsync(),
                ByDuration.Day => await baseQuery.GroupBy(p => p.TimeStamp.Date).CountAsync(),
                _ => await baseQuery.CountAsync()
            };

            var data = await query
                .OrderByDescending(x => x.Date)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return Ok(new { count, data });
        }

        [HttpGet(template: "latest_data")]
        public async Task<IActionResult> GetLatestData()
        {
            // First, get the latest record for each station
            var latestRecords = await context.PipesData
                .Include(x => x.Station)
                .GroupBy(x => x.StationId)
                .Select(g => g.OrderByDescending(x => x.TimeStamp).First())
                .ToListAsync();

            // Then, process each record to calculate hourly and daily totals
            var result = new List<PipesToReturnDto>();

            foreach (var record in latestRecords)
            {
                var lastHour = record.TimeStamp.AddHours(-1);
                var lastDay = record.TimeStamp.AddDays(-1);

                var hourlyDischarge = await context.PipesData
                    .Where(x => x.StationId == record.StationId
                        && x.TimeStamp >= lastHour
                        && x.TimeStamp <= record.TimeStamp)
                    .SumAsync(x => x.Discharge ?? 0);

                var dailyDischarge = await context.PipesData
                    .Where(x => x.StationId == record.StationId
                        && x.TimeStamp >= lastDay
                        && x.TimeStamp <= record.TimeStamp)
                    .SumAsync(x => x.Discharge ?? 0);

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

            return Ok(result);
        }

        [HttpGet("byMinute")]
        public async Task<IActionResult> GetHourlyRecords(long stationId, DateTime date)
        {
            var byMinuteRecords = await context.PipesData.Where(s =>
                    s.StationId == stationId && s.TimeStamp.Date == DateTime.SpecifyKind(date.Date, DateTimeKind.Utc))
                .OrderByDescending(s => s.TimeStamp).ToListAsync();

            return Ok(byMinuteRecords);
        }

        [HttpGet("Station/{id}")]
        public async Task<ActionResult<IEnumerable<PipesData>>> GetPipeDataByStationId(long id)
        {
            return await context.PipesData.Where(s => s.StationId == id).OrderByDescending(s => s.TimeStamp).ToListAsync();
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<PipesData>> BulkPipesData([FromBody] List<PipesDataDto> pipesData)
        {
            var pipes = pipesData.Select((pipeDto, index) => new PipesData
            {
                TimeStamp = DateTime.UtcNow.AddMinutes(index),
                StationId = pipeDto.StationId,
                Record = pipeDto.Record,
                Discharge = pipeDto.Discharge,
                TotalVolumePerHour = pipeDto.TotalVolumePerHour,
                TotalVolumePerDay = pipeDto.TotalVolumePerDay,
                Pressure = pipeDto.Pressure,
                CL = pipeDto.CL,
                Turbidity = pipeDto.Turbidity,
                ElectricConductivity = pipeDto.ElectricConductivity
            }).ToList();

            context.AddRange(pipes);
            await context.SaveChangesAsync();

            return Created();
        }
    }
}
