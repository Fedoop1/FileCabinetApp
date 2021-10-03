using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace FileCabinetGenerator
{
    public static class Program
    {
        private static IConfiguration configuration;

        private static void Configure(string[] commandLineArgs)
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("generation-settings.json")
                .AddCommandLine(commandLineArgs, new Dictionary<string, string>
                {
                    ["-t"] = "OutputType",
                    ["-o"] = "FilePath",
                    ["-a"] = "RecordsAmount",
                    ["-i"] = "StartId",
                    ["--output-type"] = "OutputType",
                    ["--output"] = "FilePath",
                    ["--records-amount"] = "RecordsAmount",
                    ["--start-id"] = "StartId"
                })
                .Build();
        }

        private static ExportService ConfigureExportService(GenerationSettings settings)
        {
            var result = new ExportService(settings);

            result.AddExportProvider("xml", filepath => new XmlRecordExporter(filepath));
            result.AddExportProvider("csv", filepath => new CsvRecordExporter(filepath));

            return result;
        }

        private static GenerationSettings SetupGenerationSettings() => configuration.Get<GenerationSettings>();

        private static void DisplayCommandLineData(string[] args)
        {
            foreach (var commandLineArg in args)
            {
                Console.Write(commandLineArg + " ");
            }
        }


        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("$FileCabinetGenerator");
                DisplayCommandLineData(args);
                Configure(args);
                var settings = SetupGenerationSettings();
                var generatedRecords = RecordGenerator.GenerateRecord(settings);
                var exportService = ConfigureExportService(settings);
                exportService.Export(generatedRecords);

                Console.WriteLine($"Export complete. {settings.RecordsAmount} record(s) were exported in {settings.OutputType} format to {settings.FilePath} with start id = {settings.StartId}.");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An exception happened during generator work:\nException message: {exception.Message}");
            }
        }
    }
}
