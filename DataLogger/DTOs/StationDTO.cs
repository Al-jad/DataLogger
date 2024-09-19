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
}