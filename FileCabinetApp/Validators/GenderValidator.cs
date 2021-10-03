using System;
using System.Linq;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class which validate incoming gender value.
    /// </summary>
    public class GenderValidator : IRecordValidator
    {
        /// <summary>
        /// An array of valid values for gender.
        /// </summary>
        private readonly char[] validGenderArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenderValidator"/> class.
        /// </summary>
        /// <param name="validGenderArray">An array of valid values for gender.</param>
        public GenderValidator(char[] validGenderArray)
        {
            this.validGenderArray = validGenderArray;
        }

        /// <inheritdoc/>
        public bool ValidateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record data can't be null");
            }

            return this.validGenderArray.Contains(record.Gender);
        }
    }
}
