using System;
using System.Linq;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Check the incoming last name value.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private readonly int minNameLength;
        private readonly int maxNameLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="minNameLength">The minimum length of the last name.</param>
        /// <param name="maxNameLength">The maximum length of the last name.</param>
        public LastNameValidator(int minNameLength, int maxNameLength)
        {
            this.minNameLength = minNameLength;
            this.maxNameLength = maxNameLength;
        }

        /// <inheritdoc/>
        public bool ValidateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record data is null");
            }

            return record.FirstName.Length > this.minNameLength && record.FirstName.Length < this.maxNameLength &&
                   !record.FirstName.Any(char.IsNumber);
        }
    }
}
