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
        /// <summary>
        /// The minimum length of the last name.
        /// </summary>
        private readonly int minNameLength;

        /// <summary>
        /// The maximum length of the last name.
        /// </summary>
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
        public void ValidateParameters(FileCabinetRecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "Record data is null");
            }

            if (recordData.FirstName.Length < this.minNameLength || recordData.FirstName.Length > this.maxNameLength || recordData.FirstName.Any(symbol => char.IsNumber(symbol)))
            {
                throw new ArgumentException("Last name is incorrect.");
            }
        }
    }
}
