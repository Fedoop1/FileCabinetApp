using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Represent a instance of record query.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Interfaces.IRecordQuery" />
    /// <seealso cref="RecordQuery" />
    public record RecordQuery : IRecordQuery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordQuery"/> class.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="hashCode">Hash code.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throws when predicate is null
        /// or
        /// Throws when hash code is null.
        /// </exception>
        public RecordQuery(Predicate<FileCabinetRecord> predicate, string hashCode) =>
            (this.Predicate, this.QueryHashCode) = (
                predicate ?? throw new ArgumentNullException(nameof(predicate), "Predicate can't be null"),
                hashCode ?? throw new ArgumentNullException(nameof(hashCode), "Hash code can't be null"));

        /// <inheritdoc/>
        public Predicate<FileCabinetRecord> Predicate { get; }

        /// <inheritdoc/>
        public string QueryHashCode { get; }
    }
}
