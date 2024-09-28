using DataLoggerDatabase.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using DataLogger.DTOs;
using DataLoggerDatabase.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DataLogger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PipesController(AppDbContext context) : ControllerBase
    {
        [HttpGet(template:"latest_data")]
        public async Task<IActionResult> GetLatestData()
        {
            var stations = await context.Stations.ToListAsync();
            var result = new List<object>();
            foreach (var station in stations)
            {
                var latestPipesData = await context.PipesData
                    .Where(x => x.StationId == station.Id)
                    .OrderByDescending(x => x.TimeStamp)
                    .FirstOrDefaultAsync();

                if (latestPipesData != null) result.Add(latestPipesData);
            }
            return Ok(result);
        }
        
        [HttpGet("byMinute")]
        public async Task<IActionResult> GetHourlyRecords(long stationId, DateTime date)
        {
            var byMinuteRecords = await context.PipesData.Where(s =>
                    s.StationId == stationId && s.TimeStamp.HasValue && s.TimeStamp.Value.Date == date.Date)
                .OrderByDescending(s => s.TimeStamp).ToListAsync();
        
            return Ok(byMinuteRecords);
        }
        [HttpGet("hourly")]
        public async Task<IActionResult> GetHourlyRecords()
        {
            var result = new List<object>(); // Holds the final result

            var stations = await context.Stations.ToListAsync(); // Get all stations

            foreach (var station in stations)
            {
                var hourlyRecords = await context.PipesData
                    .Where(p => p.StationId == station.Id && p.TimeStamp.HasValue)
                    .GroupBy(p => new
                    {
                        p.TimeStamp.Value.Year,
                        p.TimeStamp.Value.Month,
                        p.TimeStamp.Value.Day,
                        p.TimeStamp.Value.Hour
                    })
                    .Select(g => g.OrderBy(p => p.TimeStamp).FirstOrDefault()) // Select the first record per group
                    .ToListAsync();

                // Create the structure with station and pipesData
                var stationData = new
                {
                    station = station,  // Station details
                    pipesData = hourlyRecords.Select(r => new 
                    {
                        pipe = r  // Pipe data per hour
                    }).ToList()
                };

                result.Add(stationData); // Add to the result
            }

            return Ok(result); // Return the final result as JSON
        }        
        [HttpGet("Station/{id}")]
        public async Task<ActionResult<IEnumerable<PipesData>>> GetPipeDataByStationId(long id)
        {
            return await context.PipesData.Where(s => s.StationId == id).OrderByDescending(s => s.TimeStamp).ToListAsync();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PipesData>>> GetPipesData()
        {
            return await context.PipesData.ToListAsync();
        }
        [HttpGet("{id}")]
        public ActionResult<PipesData> GetPipesDataById(long id)
        {
            var pipe = context.PipesData.FirstOrDefault(s => s.Id == id);
            if (pipe == null) return NotFound();
            return Ok(pipe);
        }
        [HttpPost]
        public async Task<ActionResult<PipesData>> PostPipesData([FromBody] PipesDataDto pipeDto)
        {
            var pipes = await context.PipesData.ToListAsync();
            // var station = context.Stations.FirstOrDefault(s => s.Id == pipeDto.StationId);
            var pipe = new PipesData
            {
                Id = pipes.Count > 0 ? pipes.Max(s => s.Id) + 1 : 1,
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
    }
}
