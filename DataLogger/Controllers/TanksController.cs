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
    public class TanksController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TankData>>> GetTanksData()
        {
            return await context.TankData.ToListAsync();
        }
        [HttpGet("{id}")]
        public ActionResult<TankData> GetTankDataById(long id)
        {
            var tank = context.TankData.FirstOrDefault(s => s.Id == id);
            if (tank == null) return NotFound();
            return Ok(tank);
        }
        [HttpPost]
        public async Task<ActionResult<TankData>> PostTankData([FromBody] TankDataDto tankDto)
        {
            var tanks = await context.TankData.ToListAsync();
        
            var tank = new TankData
            {
                Id = tanks.Count > 0 ? tanks.Max(s => s.Id) + 1 : 1,
                TimeStamp = DateTime.UtcNow,
                Record = tankDto.Record,
                TotalVolumePerHour = tankDto.TotalVolumePerHour,
                TotalVolumePerDay = tankDto.TotalVolumePerDay,
                Turbidity = tankDto.Turbidity,
                ElectricConductivity = tankDto.ElectricConductivity
            };
        
            context.TankData.Add(tank);
            await context.SaveChangesAsync();            
            return CreatedAtAction(nameof(GetTankDataById), new { id = tank.Id }, tank);
        } 
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTankDataById(long id, [FromBody] TankDataDto updatedTankDataDto)
        {
            var tank = context.TankData.FirstOrDefault(s => s.Id == id);
            if (tank == null) return NotFound();
        
            tank.Record = updatedTankDataDto.Record;
            tank.TotalVolumePerHour = updatedTankDataDto.TotalVolumePerHour;
            tank.TotalVolumePerDay = updatedTankDataDto.TotalVolumePerDay;
            tank.Turbidity = updatedTankDataDto.Turbidity;
            tank.ElectricConductivity = updatedTankDataDto.ElectricConductivity;
            
            context.Entry(tank).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
