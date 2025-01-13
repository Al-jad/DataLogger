namespace DataLogger.DTOs
{
    public class TankDataDto
    {
        public long StationId { get; set; }
        public int Record { get; set; }
        public float? WL { get; set; }
        public float? TotalVolumePerHour { get; set; }
        public float? TotalVolumePerDay { get; set; }
        public float? Turbidity { get; set; }
        public float? ElectricConductivity { get; set; }
    }
    
    public class TankDataToReturnDto
    {
        public long StationId { get; set; }
        public int Record { get; set; }
        public float? WaterLevel { get; set; }
        public float? TankSensorReading { get; set; }
        public float? TankCurrentVol { get; set; }
    }
}