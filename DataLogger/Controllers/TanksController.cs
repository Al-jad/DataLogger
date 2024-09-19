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
        [HttpGet("Station/{id}")]
        public async Task<ActionResult<IEnumerable<TankData>>> GetTankDataByStationId(long id)
        {
            return await context.TankData.Where(s => s.StationId == id).OrderByDescending(s => s.TimeStamp).ToListAsync();
        }
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
            // var station = context.Stations.FirstOrDefault(s => s.Id == tankDto.StationId);

            var tank = new TankData
            {
                Id = tanks.Count > 0 ? tanks.Max(s => s.Id) + 1 : 1,
                StationId = tankDto.StationId,
                TimeStamp = DateTime.UtcNow,
                // Station = station,
                Record = tankDto.Record,
                TotalVolumePerHour = tankDto.TotalVolumePerHour,
                TotalVolumePerDay = tankDto.TotalVolumePerDay,
                WL = tankDto.WL,
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
            // var station = context.Stations.FirstOrDefault(s => s.Id == updatedTankDataDto.StationId);

            if (tank == null) return NotFound();
            tank.StationId = updatedTankDataDto.StationId;
            tank.Record = updatedTankDataDto.Record;
            tank.TotalVolumePerHour = updatedTankDataDto.TotalVolumePerHour;
            // tank.Station = station;
            tank.TotalVolumePerDay = updatedTankDataDto.TotalVolumePerDay;
            tank.Turbidity = updatedTankDataDto.Turbidity;
            tank.WL = updatedTankDataDto.WL;
            tank.ElectricConductivity = updatedTankDataDto.ElectricConductivity;
            
            context.Entry(tank).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
