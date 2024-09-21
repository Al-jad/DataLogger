using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace DataLoggerDatabase.Models;

public sealed class TankDataMap : ClassMap<TankData>
{
    public TankDataMap()
    {
        Map(m => m.TimeStamp).Name("TIMESTAMP");

        // Map(m => m.TimeStamp).Convert(row => DateTime.Parse(row.Row.GetField(0)).AddHours(-3).ToUniversalTime());
        
        Map(m => m.TotalVolumePerHour).Name("TotalVol(h)");
        Map(m => m.TotalVolumePerDay).Name("TotalVol(d)");
        Map(m => m.Turbidity).Name("Turbidity");
        Map(m => m.ElectricConductivity).Name("EC");
        Map(m => m.WL).Name("WL");
        
        
    }
    
    public static List<TankData> ParseCsvFile(string filePath)
    {
        try
        {
            var conf = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // BadDataFound = null,
                // HeaderValidated = null,
                // MissingFieldFound = null
            };
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, conf))
            {
                csv.Context.RegisterClassMap<TankDataMap>();
                var records = csv.GetRecords<TankData>();
                return Enumerable.ToList<TankData>(records);
            }
        }
        catch (HeaderValidationException ex)
        {
            throw new ApplicationException("CSV file header is invalid.", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error reading CSV file", ex);
        }
    }
    
}