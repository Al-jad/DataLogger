using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataLogger.Models;


namespace DataLogger.Controllers;

[Route("API/[controller]")]
[ApiController]
public class StationsController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Station>>> GetStations()
    {
        return await context.Stations.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Station>> GetStations(int id)
    {
        var stationInstance = await context.Stations.FindAsync(id);

        if (stationInstance == null)
        {
            return NotFound();
        }

        return stationInstance;
    }

    [HttpPost]
    public async Task<ActionResult<Station>> PostStation(Station station)
    {
        station.CreatedAt = DateTime.Now;
        context.Stations.Add(station);
        await context.SaveChangesAsync();

        return CreatedAtAction("PostStation", new { id = station.Id }, station);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutStation(int id, Station station)
    {
        if (id != station.Id)
        {
            return BadRequest();
        }

        context.Entry(station).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StationExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStation(int id)
    {
        var station = await context.Stations.FindAsync(id);

        if (station == null)
        {
            return NotFound();
        }

        context.Stations.Remove(station);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool StationExists(int id)
    {
        return context.Stations.Any(e => e.Id == id);
    }
}