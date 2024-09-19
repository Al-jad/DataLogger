using DataLogger.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using DataLogger.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DataLogger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationsController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Station>>> GetStations()
        {
            return await context.Stations.ToListAsync();
        }
        [HttpGet("{id}")]
        public ActionResult<Station> GetStationById(long id)
        {
            var station = context.Stations.FirstOrDefault(s => s.Id == id);
            if (station == null) return NotFound();
            return Ok(station);
        }
        [HttpPost]
        public async Task<ActionResult<Station>> PostStation([FromBody] StationDto stationDto)
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
                SourceAddress = stationDto.SourceAddress
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
    }
}
