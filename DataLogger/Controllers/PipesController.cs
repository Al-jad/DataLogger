using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataLogger.Models;


namespace DataLogger.Controllers;

[Route("API/[controller]")]
[ApiController]
public class PipesController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PipesData>>> GetPipes()
    {
        return await context.PipesData.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PipesData>> GetPipe(int id)
    {
        var pipeDataInstance = await context.PipesData.FindAsync(id);

        if (pipeDataInstance == null)
        {
            return NotFound();
        }

        return pipeDataInstance;
    }

    [HttpPost]
    public async Task<ActionResult<PipesData>> PostPipe(PipesData pipe)
    {
        pipe.TimeStamp = DateTime.Now;
        context.PipesData.Add(pipe);
        await context.SaveChangesAsync();

        return CreatedAtAction("PostPipe", new { id = pipe.Id }, pipe);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPipe(int id, PipesData pipe)
    {
        if (id != pipe.Id)
        {
            return BadRequest();
        }

        context.Entry(pipe).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PipeDataExists(id))
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
    public async Task<IActionResult> DeletePipe(int id)
    {
        var pipe = await context.PipesData.FindAsync(id);

        if (pipe == null)
        {
            return NotFound();
        }

        context.PipesData.Remove(pipe);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool PipeDataExists(int id)
    {
        return context.PipesData.Any(e => e.Id == id);
    }
}