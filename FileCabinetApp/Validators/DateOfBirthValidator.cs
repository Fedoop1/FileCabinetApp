using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class which validate incoming date of birth value.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        /// <summary>
        /// The minimum allowed date of birth.
        /// </summary>
        private readonly DateTime minDateOfBirth;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="minDateOfBirth">The minimum allowed date of birth.</param>
        public DateOfBirthValidator(DateTime minDateOfBirth)
        {
            this.minDateOfBirth = minDateOfBirth;
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "Record data is null");
            }

            if (recordData.DateOfBirth < this.minDateOfBirth || recordData.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date of birth is incorrect.");
            }
        }
    }
}
