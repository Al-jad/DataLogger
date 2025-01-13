using DataLoggerDatabase.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DataLoggerDatabase;

using System.Linq;
using DataLogger.DTOs;
using DataLoggerDatabase.Migrations;
using Microsoft.EntityFrameworkCore;

namespace DataLogger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationsController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Station>>> GetStations(Enums.StationType stationType)
        {
            return await context.Stations
                .Where(s => s.StationType == stationType)
                .ToListAsync();
        }
        [HttpGet("{id}")]
        public ActionResult<Station> GetStationById(long id)
        {
            var station = context.Stations
                .Where(s => s.Id == id)
                .Select(s => new StationToReturnDto
                {
                    Name = s.Name,
                    City = s.City,
                    DataFile = s.DataFile,
                    UploadedDataFile = s.UploadedDataFile,
                    Lat = s.Lat,
                    Lng = s.Lng,
                    Description = s.Description,
                    Notes = s.Notes,
                    SourceAddress = s.SourceAddress,
                    StationType = s.StationType
                })
                .FirstOrDefault()
                ;
            if (station == null) return NotFound();
            
            
            return Ok(station);
        }
        [HttpPost]
        public async Task<ActionResult<Station>> PostStation([FromBody] StationDto stationDto, Enums.StationType stationType)
        {
            var stations = await context.Stations.ToListAsync();
        
            var station = new Station
            {
                Id = stations.Count > 0 ? stations.Max(s => s.Id) + 1 : 1,
                CreatedAt = DateTime.UtcNow,
                Name = stationDto.Name,
                City = stationDto.City,
                DataFile = stationDto.DataFile,
                UploadedDataFile = stationDto.UploadedDataFile,
                Lat = stationDto.Lat,
                Lng = stationDto.Lng,
                Description = stationDto.Description,
                Notes = stationDto.Notes,
                SourceAddress = stationDto.SourceAddress,
                StationType = stationType
            };
        
            context.Stations.Add(station);
            await context.SaveChangesAsync();            
            return CreatedAtAction(nameof(GetStationById), new { id = station.Id }, station);
        } 
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStationById(long id, [FromBody] StationDto updatedStationDto)
        {
            var station = context.Stations.FirstOrDefault(s => s.Id == id);
            if (station == null) return NotFound();
        
            station.Name = updatedStationDto.Name;
            station.City = updatedStationDto.City;
            station.DataFile = updatedStationDto.DataFile;
            station.UploadedDataFile = updatedStationDto.UploadedDataFile;
            station.Lat = updatedStationDto.Lat;
            station.Lng = updatedStationDto.Lng;
            station.Description = updatedStationDto.Description;
            station.Notes = updatedStationDto.Notes;
            station.SourceAddress = updatedStationDto.SourceAddress;
            context.Entry(station).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("{id}/tank")]
        public async Task<IActionResult> PutTankStationById(long id, [FromBody] PipeStationDto updatedStationDto)
        {
            var station = context.Stations.Where(s => s.StationType == Enums.StationType.Tank).FirstOrDefault(s => s.Id == id);
            if (station == null) return NotFound();
        
            station.BaseArea = updatedStationDto.BaseArea ?? station.BaseArea;
            station.TankHeight = updatedStationDto.TankHeight ?? station.TankHeight;
            context.Entry(station).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
