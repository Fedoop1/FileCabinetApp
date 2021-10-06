using System;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Expose record query contract.
    /// </summary>
    public interface IRecordQuery
    {
        /// <summary>
        /// Gets the predicate according to select query.
        /// </summary>
        /// <value>
        /// Predicate instance.
        /// </value>
        public Predicate<FileCabinetRecord> Predicate { get; }

        /// <summary>
        /// Gets the hash code of the query.
        /// </summary>
        /// <value>
        /// Query's hash code.
        /// </value>
        public string QueryHashCode { get; }
    }
}
