using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class which validate incoming date of birth value.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime minDateOfBirth;
        private readonly DateTime maxDateOfBirth = DateTime.Now;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="minDateOfBirth">The minimum allowed date of birth.</param>
        public DateOfBirthValidator(DateTime minDateOfBirth)
        {
            this.minDateOfBirth = minDateOfBirth;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="minDateOfBirth">The minimum date of birth.</param>
        /// <param name="maxDateOfBirth">The maximum date of birth.</param>
        public DateOfBirthValidator(DateTime minDateOfBirth, DateTime maxDateOfBirth)
        {
            this.minDateOfBirth = minDateOfBirth;
            this.maxDateOfBirth = maxDateOfBirth;
        }

        /// <inheritdoc/>
        public bool ValidateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record can't be null");
            }

            return record.DateOfBirth > this.minDateOfBirth && record.DateOfBirth < this.maxDateOfBirth;
        }
    }
}
