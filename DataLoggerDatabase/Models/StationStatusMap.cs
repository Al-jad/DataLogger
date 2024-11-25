using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;

namespace DataLoggerDatabase.Models;

public sealed class StationStatusMap : ClassMap<StationStatus>
{
    public StationStatusMap()
    {
        Map(m => m.TimeStamp).Convert(row => DateTime.Parse(row.Row.GetField(0)).AddHours(-3).ToUniversalTime());

        // Map(m => m.TimeStamp).Name("TMSTAMP");
        Map(m => m.Record).Name("RECNBR");
        Map(m => m.StartUpCode).Name("StartUpCode");
    }

    public static List<StationStatus> ParseCsvFile(string filePath)
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
            csv.Context.RegisterClassMap<StationStatusMap>();
            var records = csv.GetRecords<StationStatus>();
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