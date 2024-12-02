using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace DataLoggerDatabase.Models;

public sealed class PipesDataMap : ClassMap<PipesData>
{
    public PipesDataMap()
    {
        // Map(m => m.TimeStamp).Name("TIMESTAMP");
        Map(m => m.TimeStamp).Convert(row => DateTime.SpecifyKind(DateTime.Parse(row.Row.GetField(0)!).AddHours(-3), DateTimeKind.Utc));
        Map(m => m.Discharge).Name("Discharge1");
        Map(m => m.Discharge2).Name("Discharge2");
        Map(m => m.Temperature).Name("PTemp_C_Avg");
        // Map(m => m.BatteryVoltage).Name("BattV");
        Map(m => m.Record).Name("RECNBR");
        // Map(m => m.TotalVolumePerHour).Name("TotalVol(h)");
        // Map(m => m.TotalVolumePerDay).Name("TotalVol(d)");
        Map(m => m.Pressure).Name("Pressure1_Avg");
        Map(m => m.Pressure2).Name("Pressure2_Avg");
        // Map(m => m.CL).Name("Chlorine");
        // Map(m => m.Turbidity).Name("Turbidity");
        // Map(m => m.ElectricConductivity).Name("EC");
    }
    
    public static List<PipesData> ParseCsvFile(string filePath)
    {
        try
        {
            var conf = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // BadDataFound = null,
                // HeaderValidated = null,
                // MissingFieldFound = null
            };
            using var reader = new StreamReader(filePath);

            const int linesToSkip = 1;
            for (var i = 0; i < linesToSkip; i++)
            {
                reader.ReadLine();
            }
            using var csv = new CsvReader(reader, conf);
            csv.Context.RegisterClassMap<PipesDataMap>();
            var records = csv.GetRecords<PipesData>();
            return records.ToList();
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