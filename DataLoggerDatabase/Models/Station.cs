using System.ComponentModel.DataAnnotations;

namespace DataLoggerDatabase.Models;

public class Station
{
    public long Id { get; set; }
    public string? ExternalId { get; set; } = null!;
    public required string Name { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    [StringLength(40)] public string? City { get; set; }
    
    // public List<PipesData> PipesData { get; set; } = [];
    // public List<TankData> TankData { get; set; } = [];
    public string? DataFile { get; set; }
    public string? UploadedDataFile { get; set; }

    public double? Lat { get; set; }
    public double? Lng { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public string? SourceAddress { get; set; }
    
    
}