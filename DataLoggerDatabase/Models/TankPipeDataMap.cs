using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace DataLoggerDatabase.Models
{
    public class TankPipeDataMap : ClassMap<PipesData>
    {
        public TankPipeDataMap()
        {
            //"TMSTAMP","RECNBR","BattV","PTemp_C","Level"
            // Map(m => m.TimeStamp).Name("TIMESTAMP");
            Map(m => m.TimeStamp).Convert(row => DateTime.SpecifyKind(DateTime.Parse(row.Row.GetField(0)!).AddHours(-3), DateTimeKind.Utc));
            Map(m => m.Record).Name("RECNBR");
            Map(m => m.BatteryVoltage).Name("BattV");
            Map(m => m.Temperature).Name("PTemp_C");
            Map(m => m.WaterLevel).Name("Level");
        }

        public static List<PipesData> ParseTankCsvFile(string filePath)
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
                csv.Context.RegisterClassMap<TankPipeDataMap>();
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
}
