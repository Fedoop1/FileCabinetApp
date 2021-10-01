using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Snapshot class with private information about list of <see cref="FileCabinetRecord"/> required for secure serialization.
    /// </summary>
    public class FileCabinetSnapshotService : IRecordSnapshotService
    {
        private readonly Dictionary<string, Func<string, object>> dataSaverProviders =
            new (StringComparer.CurrentCultureIgnoreCase);

        private readonly Dictionary<string, Func<string, object>> dataLoaderProviders =
            new(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetSnapshotService"/> class and save information and assign inner field by reference to an array with records.
        /// </summary>
        /// <param name="source"><see cref="FileCabinetRecord"/> array with data about records.</param>
        public FileCabinetSnapshotService(IEnumerable<FileCabinetRecord> source) => this.Records = source ?? throw new ArgumentNullException(nameof(source), "Source can't be null");

        /// <summary>
        /// Gets <see cref="ReadOnlyCollection{T}"/> of <see cref="FileCabinetRecord"/> which is stored into snapshot.
        /// </summary>
        /// <value>
        /// Read only collection of <see cref="FileCabinetRecord"/>.
        /// </value>
        public IEnumerable<FileCabinetRecord> Records { get; private set; }

        public void SaveTo(string fileFormat, string filePath, bool append)
        {
            if (!this.dataSaverProviders.TryGetValue(fileFormat, out var storageCreator) || storageCreator.Invoke(filePath) is not IRecordDataSaver saver)
            {
                throw new ArgumentException("File format doesn't support");
            }

            saver.Save(this.Records, append);
        }

        public void LoadFrom(string fileFormat, string filePath)
        {
            if (!this.dataLoaderProviders.TryGetValue(fileFormat, out var storageLoader) || storageLoader.Invoke(filePath) is not IRecordDataLoader loader)
            {
                throw new ArgumentException("File format doesn't support");
            }

            this.Records = loader.Load();
        }

        public void AddDataSaver(string fileFormat, Func<string, object> creator)
        {
            if (string.IsNullOrEmpty(fileFormat))
            {
                throw new ArgumentNullException(nameof(fileFormat), "File format can't be null or empty");
            }

            this.dataSaverProviders.Add(fileFormat, creator);
        }

        public void AddDataLoader(string fileFormat, Func<string, object> creator)
        {
            if (string.IsNullOrEmpty(fileFormat))
            {
                throw new ArgumentNullException(nameof(fileFormat), "File format can't be null or empty");
            }

            this.dataLoaderProviders.Add(fileFormat, creator);
        }
    }
}
