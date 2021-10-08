using System.Collections.Generic;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Determine methods to services that load data from storage.
    /// </summary>
    public interface IRecordDataLoader
    {
        /// <summary>
        /// Loads data from storage.
        /// </summary>
        /// <returns>Sequence of load data.</returns>
        public IEnumerable<FileCabinetRecord> Load();
    }
}
