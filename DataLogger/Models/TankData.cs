namespace DataLogger.Models;

public class TankData
{
    public long Id { get; set; }
    public DateTime? TimeStamp { get; set; }
    public int Record { get; set; }
    public long StationId { get; set; }
    public Station? Station { get; set; }
    
    public float? WL { get; set; }
    public float? TotalVolumePerHour { get; set; }
    public float? TotalVolumePerDay { get; set; }
    public float? Turbidity { get; set; }
    public float? ElectricConductivity { get; set; }
    
}