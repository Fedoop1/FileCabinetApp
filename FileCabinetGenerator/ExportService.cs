using System;
using System.Collections.Generic;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Export service which save data source to destination file according to specified type.
    /// </summary>
    public class ExportService
    {
        private readonly Dictionary<string, Func<string, object>> exportProviders =
            new (StringComparer.CurrentCultureIgnoreCase);

        private readonly GenerationSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportService"/> class.
        /// </summary>
        /// <param name="settings">Export settings.</param>
        /// <exception cref="ArgumentNullException">Throws when export settings is null.</exception>
        public ExportService(GenerationSettings settings) => this.settings = settings??
            throw new ArgumentNullException(nameof(settings), "Generation settings is null");


        /// <summary>
        /// Exports <see cref="FileCabinetRecord"/> source to destination file.
        /// </summary>
        /// <param name="source">The records source.</param>
        /// <exception cref="ArgumentException">Throws when export format is null or empty.</exception>
        public void Export(IEnumerable<FileCabinetRecord> source)
        {
            if (!this.exportProviders.TryGetValue(settings.OutputType, out var creator) || creator.Invoke(settings.FilePath) is not IRecordExporter exporter)
            {
                throw new ArgumentException("Export format doesn't support");
            }

            exporter.Export(source);
        }


        /// <summary>
        /// Adds new <see cref="IRecordExporter"/> provider.
        /// </summary>
        /// <param name="fileFormat">The file format.</param>
        /// <param name="creator">The creator function.</param>
        /// <exception cref="ArgumentNullException">Throws when file format is null or empty.</exception>
        public void AddExportProvider(string fileFormat, Func<string, IRecordExporter> creator)
        {
            if (string.IsNullOrEmpty(fileFormat))
            {
                throw new ArgumentNullException(nameof(fileFormat), "Export format is null or empty");
            }

            this.exportProviders.Add(fileFormat, creator);
        }
    }
}
