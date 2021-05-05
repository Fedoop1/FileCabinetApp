using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void ValidateParameters(FileCabinetRecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "Record data is null");
            }

            if (!this.validGenderArray.Contains(recordData.Gender))
            {
                throw new ArgumentException("Gender is incorrect.");
            }
        }
    }
}
