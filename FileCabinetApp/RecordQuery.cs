using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Interfaces;

#pragma warning disable CA1308 // Normalize strings to uppercase

namespace FileCabinetApp
{
    /// <summary>
    /// Represent a instance of record query.
    /// </summary>
    /// <seealso cref="IRecordQuery" />
    /// <seealso cref="RecordQuery" />
    public record RecordQuery : IRecordQuery
    {
        private string hashCode;
        private readonly string queryString;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordQuery"/> class.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="queryString">Hash code.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throws when predicate is null
        /// or
        /// Throws when hash code is null.
        /// </exception>
        public RecordQuery(Predicate<FileCabinetRecord> predicate, string queryString)
        {
            this.Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate), "Predicate can't be null");
            this.queryString = queryString ?? throw new ArgumentNullException(nameof(queryString), "Query string can't be null");
        }

        /// <inheritdoc/>
        public Predicate<FileCabinetRecord> Predicate { get; }

        /// <inheritdoc/>
        public string QueryHashCode
        {
            get
            {
                if (this.hashCode != default)
                {
                    return this.hashCode;
                }

                this.hashCode = CalculateHashCode(this.queryString);
                return this.hashCode;
            }
        }

        private static string CalculateHashCode(string queryString)
        {
            if (!queryString.Contains("where", StringComparison.CurrentCultureIgnoreCase))
            {
                return string.Empty;
            }

            var whereArray = queryString.ToLowerInvariant().Split(
                "where",
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            var keyValuePair =

                CommandHandlerExtensions.ExtractKeyValuePair(whereArray[^1].ToLowerInvariant(), new[] { "and" });

            return GenerateHashCode(keyValuePair);
        }

        private static string GenerateHashCode(SortedDictionary<string, string> keyValuePair) =>
            string.Join(string.Empty, keyValuePair.Select(pair => (pair.Key + pair.Value).ToUpperInvariant()));
    }
}
