using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    public class FirstNameValidator : IRecordValidator
    {
        /// <summary>
        /// The minimum length of the first or last name.
        /// </summary>
        private readonly int minNameLength;

        /// <summary>
        /// The maximum length of a given name or surname.
        /// </summary>
        private readonly int maxNameLength;

        public FirstNameValidator(int minNameLength, int maxNameLength)
        {
            this.minNameLength = minNameLength;
            this.maxNameLength = maxNameLength;
        }

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
