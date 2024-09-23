using DataLoggerDatabase.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using DataLogger.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DataLogger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PipesController(AppDbContext context) : ControllerBase
    {
        [HttpGet("hourly")]
        public async Task<IActionResult> GetHourlyRecords(long stationId, DateTime date)
        {
            var hourlyRecords = await context.PipesData.Where(s => s.StationId == stationId && s.TimeStamp.HasValue && s.TimeStamp.Value.Date == date.Date)
                .GroupBy(r => new
                {
                    r.TimeStamp.Value.Year,
                    r.TimeStamp.Value.Month,
                    r.TimeStamp.Value.Day,
                    r.TimeStamp.Value.Hour
                    
                })
                .Select(g => g.OrderBy(r => r.TimeStamp).FirstOrDefault())
                .ToListAsync();

            return Ok(hourlyRecords);
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
