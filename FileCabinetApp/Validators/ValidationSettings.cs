using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class that contains all boundary values to validation rules.
    /// </summary>
    /// <seealso cref="IValidationSettings" />
    public sealed class ValidationSettings : IValidationSettings
    {
        /// <inheritdoc/>
        public int FirstName_Min { get; set; }

        /// <inheritdoc/>
        public int FirstName_Max { get; set; }

        /// <inheritdoc/>
        public int LastName_Min { get; set; }

        /// <inheritdoc/>
        public int LastName_Max { get; set; }

        /// <inheritdoc/>
        public DateTime DateOfBirth_From { get; set; }

        /// <inheritdoc/>
        public DateTime DateOfBirth_To { get; set; }

        /// <inheritdoc/>
        public short Height_Min { get; set; }

        /// <inheritdoc/>
        public short Height_Max { get; set; }

        /// <inheritdoc/>
        public decimal Money_Min { get; set; }

        /// <inheritdoc/>
        public decimal Money_Max { get; set; }
    }
}
