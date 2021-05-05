using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class which validate incoming first name value.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        /// <summary>
        /// The minimum length of the first name.
        /// </summary>
        private readonly int minNameLength;

        /// <summary>
        /// The maximum length of a the first name.
        /// </summary>
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
        public void ValidateParameters(FileCabinetRecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "Record data is null");
            }

            if (recordData.FirstName.Length < this.minNameLength || recordData.FirstName.Length > this.maxNameLength || recordData.FirstName.Any(symbol => char.IsNumber(symbol)))
            {
                throw new ArgumentException("First name is incorrect.");
            }
        }
    }
}
