using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.DataTransfer
{
    /// <summary>
    /// Class DTO that save <see cref="IFileCabinetService"/> records.
    /// </summary>
    public class RecordSnapshot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSnapshot"/> class.
        /// </summary>
        /// <param name="records">The service records.</param>
        public RecordSnapshot(IEnumerable<FileCabinetRecord> records) => this.Records = records;

        /// <summary>
        /// Gets the saved records.
        /// </summary>
        /// <value>
        /// Records.
        /// </value>
        public IEnumerable<FileCabinetRecord> Records { get; }
    }
}
