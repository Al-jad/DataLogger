using DataLoggerDatabase.Models;

namespace DataLogger.DTOs
{
    public class PipesDataDto
    {
        public long StationId { get; set; }

        public int Record { get; set; }
        public float? Discharge { get; set; }
        public float? TotalVolumePerHour { get; set; }
        public float? TotalVolumePerDay { get; set; }
        public float? Pressure { get; set; }
        public float? CL { get; set; }
        public float? Turbidity { get; set; }
        public float? ElectricConductivity { get; set; }
    }

    public class PipesToReturnDto
    {
        public DateTime TimeStamp { get; set; }
        public long StationId { get; set; }
        public Station? Station { get; set; }

        public int Record { get; set; }
        public float? DischargeInMinute { get; set; }
        public float? DischargeInHour { get; set; }
        public float? DischargeInDay { get; set; }
        public float? TotalVolumePerHour { get; set; }
        public float? TotalVolumePerDay { get; set; }
        public float? Pressure { get; set; }
        public float? WaterLevel { get; set; }
        public float? WaterQuality { get; set; }
    
    }
}