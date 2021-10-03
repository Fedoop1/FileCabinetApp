using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Static class which export data source in CSV format to destination file.
    /// </summary>
    public class CsvRecordExporter : IRecordExporter
    {
        private readonly string filepath;

        public CsvRecordExporter(string filePath) => this.filepath = filePath ??
                                                                   throw new ArgumentNullException(nameof(filePath),
                                                                       "Destination file path is null");
        /// <summary>
        /// Export source in CSV format to destination file.
        /// </summary>
        /// <param name="source">Source</param>
        public void Export(IEnumerable<FileCabinetRecord> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source), "Records source is null");
            }

            using var textWriter = new StreamWriter(this.filepath, false);

            foreach (var record in source)
            {
                textWriter.WriteLine($"{record.Id},{record.FirstName},{record.LastName},{record.DateOfBirth},{record.Height},{record.Money},{record.Gender}");
            }
        }
    }
}
