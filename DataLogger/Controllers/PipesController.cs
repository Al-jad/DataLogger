using DataLoggerDatabase.Models;
using DataLoggerDatabase;
using Microsoft.AspNetCore.Mvc;
using DataLogger.DTOs;
using Microsoft.EntityFrameworkCore;

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
                .Where(p => p.TimeStamp.Date >= startDate.Date && p.TimeStamp.Date <= endDate.Date)
                .GroupBy(p => p.StationId)
                .Select(g => new
                {
                    StationId = g.Key,
                    DailyDischarge = g.Sum(p => p.Discharge)
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

        [HttpGet(template: "latest_data")]
        public async Task<IActionResult> GetLatestData()
        {
            var latestPipesData = await context.PipesData
                .Include(x => x.Station)
                .GroupBy(x => x.StationId)
                .Select(g => g.OrderByDescending(x => x.TimeStamp).First())
                .ToListAsync();

            return Ok(latestPipesData);
        }

        [HttpGet("byMinute")]
        public async Task<IActionResult> GetHourlyRecords(long stationId, DateTime date)
        {
            var byMinuteRecords = await context.PipesData.Where(s =>
                    s.StationId == stationId && s.TimeStamp.Date == date.Date)
                .OrderByDescending(s => s.TimeStamp).ToListAsync();

            return Ok(byMinuteRecords);
        }

        [HttpGet("hourly")]
        public async Task<IActionResult> GetHourlyRecords(long stationID)
        {


            var hourlyRecords = await context.PipesData
                .Where(p => p.StationId == stationID)
                .GroupBy(p => new
                {
                    p.TimeStamp.Year,
                    p.TimeStamp.Month,
                    p.TimeStamp.Day,
                    p.TimeStamp.Hour
                })
                .Select(g => g.OrderBy(p => p.TimeStamp).FirstOrDefault())
                .ToListAsync();

            return Ok(hourlyRecords.OrderByDescending(s => s!.TimeStamp)); // Return the final result as JSON
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
