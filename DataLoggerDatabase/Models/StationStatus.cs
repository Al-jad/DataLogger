namespace DataLoggerDatabase.Models;

public class StationStatus
{
    public long Id { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    public int Record { get; set; }

    public string? StartUpCode { get; set; }
    public long StationId { get; set; }
    public Station Station { get; set; } = null!;

}