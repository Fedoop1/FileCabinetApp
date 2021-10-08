using System;
using System.Linq;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class which validate incoming first name value.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        private readonly int minNameLength;
        private readonly int maxNameLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="minNameLength">Minimum name length value.</param>
        /// <param name="maxNameLength">Maximum name length value.</param>
        public FirstNameValidator(int minNameLength, int maxNameLength)
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
