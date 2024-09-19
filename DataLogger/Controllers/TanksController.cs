using DataLogger.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataLogger.Models;


namespace DataLogger.Controllers;

[Route("API/[controller]")]
[ApiController]
public class TanksController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TankData>>> GetTanks()
    {
        Console.WriteLine("Here");
        return await context.TankData.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TankData>> GetTanks(int id)
    {
        var tankInstance = await context.TankData.FindAsync(id);

        if (tankInstance == null)
        {
            return NotFound();
        }

        return tankInstance;
    }

    [HttpPost]
    public async Task<ActionResult<PipesData>> PostTank(TankData tank)
    {
        tank.TimeStamp = DateTime.Now;
        context.TankData.Add(tank);
        await context.SaveChangesAsync();

        return CreatedAtAction("PostTank", new { id = tank.Id }, tank);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTank(int id, TankData tank)
    {
        if (id != tank.Id)
        {
            return BadRequest();
        }

        context.Entry(tank).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TankExists(id))
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
    public async Task<IActionResult> DeleteTank(int id)
    {
        var tank = await context.TankData.FindAsync(id);

        if (tank == null)
        {
            return NotFound();
        }

        context.TankData.Remove(tank);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool TankExists(int id)
    {
        return context.TankData.Any(e => e.Id == id);
    }
}