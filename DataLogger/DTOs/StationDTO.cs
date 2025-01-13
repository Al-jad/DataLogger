using DataLoggerDatabase;

namespace DataLogger.DTOs
{
    public class StationDto
    {
        public string Name { get; set; } = null!;
        public string? City { get; set; }
        public string? DataFile { get; set; }
        public string? UploadedDataFile { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public string? SourceAddress { get; set; }
    }
    public class PipeStationDto
    {
        public float? BaseArea { get; set; }
        public float? TankHeight { get; set; }
    }
    public class StationToReturnDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? City { get; set; }
        public string? DataFile { get; set; }
        public string? UploadedDataFile { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public string? SourceAddress { get; set; }
        public Enums.StationType StationType { get; set; }
    }
}