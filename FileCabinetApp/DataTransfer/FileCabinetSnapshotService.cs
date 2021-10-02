using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.DataTransfer
{
    /// <summary>
    /// Snapshot class with private information about list of <see cref="FileCabinetRecord"/> required for secure serialization.
    /// </summary>
    public class FileCabinetSnapshotService : IRecordSnapshotService
    {
        private readonly Dictionary<string, Func<string, object>> dataSaverProviders =
            new (StringComparer.CurrentCultureIgnoreCase);

        private readonly Dictionary<string, Func<string, object>> dataLoaderProviders =
            new (StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetSnapshotService"/> class.
        /// </summary>
        /// <param name="snapshot">The snapshot.</param>
        /// <exception cref="System.ArgumentNullException">Throws when snapshot is null.</exception>
        public FileCabinetSnapshotService(RecordShapshot snapshot) => this.Records = snapshot?.Records ??
            throw new ArgumentNullException(nameof(snapshot), "Snapshot can't be null");

        /// <inheritdoc />
        public IEnumerable<FileCabinetRecord> Records { get; private set; }

        /// <inheritdoc/>
        public void SaveTo(string fileFormat, string filePath, bool append)
        {
            if (!this.dataSaverProviders.TryGetValue(fileFormat, out var storageCreator) || storageCreator.Invoke(filePath) is not IRecordDataSaver saver)
            {
                throw new ArgumentException("File format doesn't support");
            }

            saver.Save(this.Records, append);
        }

        /// <inheritdoc/>
        public void LoadFrom(string fileFormat, string filePath)
        {
            if (!this.dataLoaderProviders.TryGetValue(fileFormat, out var storageLoader) || storageLoader.Invoke(filePath) is not IRecordDataLoader loader)
            {
                throw new ArgumentException("File format doesn't support");
            }

            this.Records = loader.Load();
        }

        /// <summary>
        /// Adds new <see cref="IRecordDataSaver"/> provider.
        /// </summary>
        /// <param name="fileFormat">The file format.</param>
        /// <param name="creator">The creator function.</param>
        /// <exception cref="System.ArgumentNullException">Throws when file format is null or empty.</exception>
        public void AddDataSaver(string fileFormat, Func<string, IRecordDataSaver> creator)
        {
            if (string.IsNullOrEmpty(fileFormat))
            {
                throw new ArgumentNullException(nameof(fileFormat), "File format can't be null or empty");
            }

            this.dataSaverProviders.Add(fileFormat, creator);
        }

        /// <summary>
        /// Adds new <see cref="IRecordDataLoader"/> loader provider.
        /// </summary>
        /// <param name="fileFormat">The file format.</param>
        /// <param name="creator">The creator function.</param>
        /// <exception cref="System.ArgumentNullException">Throws when file format is null or empty.</exception>
        public void AddDataLoader(string fileFormat, Func<string, IRecordDataLoader> creator)
        {
            if (string.IsNullOrEmpty(fileFormat))
            {
                throw new ArgumentNullException(nameof(fileFormat), "File format can't be null or empty");
            }

            this.dataLoaderProviders.Add(fileFormat, creator);
        }
    }
}
