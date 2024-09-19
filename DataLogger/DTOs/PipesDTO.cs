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
}