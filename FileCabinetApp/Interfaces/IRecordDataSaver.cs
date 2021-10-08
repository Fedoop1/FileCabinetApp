using System.Collections.Generic;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Determine methods to services that save data to storage.
    /// </summary>
    public interface IRecordDataSaver
    {
        /// <summary>
        /// Save data to storage.
        /// </summary>
        /// <param name="source">The source of records.</param>
        /// <param name="append">Append storage if set to <c>true</c>, otherwise rewrite.</param>
        public void Save(IEnumerable<FileCabinetRecord> source, bool append);
    }
}
