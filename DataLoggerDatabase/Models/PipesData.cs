namespace DataLoggerDatabase.Models;

public class PipesData
{
    public long Id { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    public int Record { get; set; }

    public long StationId { get; set; }
    public Station Station { get; set; } = null!;
    public float? Discharge { get; set; }
    public float? Discharge2 { get; set; }
    public float? TotalVolumePerHour { get; set; }
    public float? TotalVolumePerDay { get; set; }
    public float? Pressure { get; set; }
    public float? Pressure2 { get; set; }
    public float? Temperature { get; set; }
    public float? BatteryVoltage { get; set; }
    public float? CL { get; set; }
    public float? Turbidity { get; set; }
    public float? ElectricConductivity { get; set; }
    public float? WaterLevel { get; set; }
    public float? WaterQuality { get; set; }
    
}