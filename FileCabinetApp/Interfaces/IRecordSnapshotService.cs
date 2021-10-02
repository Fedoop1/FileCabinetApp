using System.Collections.Generic;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Interface for services which work with <see cref="FileCabinetRecord"/> and process IO operations with them.
    /// </summary>
    public interface IRecordSnapshotService
    {
        /// <summary>
        /// Gets <see cref="FileCabinetRecord"/> records.
        /// </summary>
        /// <value></value>
        public IEnumerable<FileCabinetRecord> Records { get; }

        /// <summary>
        /// Saves records in special record format to destination file.
        /// </summary>
        /// <param name="fileFormat">The file format.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="append">if set to <c>true</c> append destination file, otherwise recreate it.</param>
        public void SaveTo(string fileFormat, string filePath, bool append);

        /// <summary>
        /// Loads records from destination file and save it.
        /// </summary>
        /// <param name="fileFormat">The file format.</param>
        /// <param name="filePath">The file path.</param>
        public void LoadFrom(string fileFormat, string filePath);
    }
}
