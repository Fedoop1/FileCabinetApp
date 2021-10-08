using System;

#pragma warning disable CA1707 // Identifiers should not contain underscores

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Interface which describe behavior of validation settings to <see cref="FileCabinetRecord"/>.
    /// </summary>
    public interface IValidationSettings
    {
        /// <summary>
        /// Gets or sets max length of first name to <see cref="FileCabinetRecord"/>.
        /// </summary>
        /// <value>Maximum length of first name.</value>
        public int FirstName_Max { get; set; }

        /// <summary>
        /// Gets or sets min length of first name to <see cref="FileCabinetRecord"/>.
        /// </summary>
        /// <value>Minimum first name length.</value>
        public int FirstName_Min { get; set; }

        /// <summary>
        /// Gets or sets max length of last name to <see cref="FileCabinetRecord"/>.
        /// </summary>
        /// <value>Maximum last name length.</value>
        public int LastName_Max { get; set; }

        /// <summary>
        /// Gets or sets min length of last name to <see cref="FileCabinetRecord"/>.
        /// </summary>
        /// <value>Minimum last name length.</value>
        public int LastName_Min { get; set; }

        /// <summary>
        /// Gets or sets minimal date of date of birth to <see cref="FileCabinetRecord"/>.
        /// </summary>
        /// <value>Minimum birth date.</value>
        public DateTime DateOfBirth_From { get; set; }

        /// <summary>
        /// Gets or sets maximum date of date of birth to <see cref="FileCabinetRecord"/>.
        /// </summary>
        /// <value>Maximum birth date.</value>
        public DateTime DateOfBirth_To { get; set; }

        /// <summary>
        /// Gets or sets min height to <see cref="FileCabinetRecord"/>.
        /// </summary>
        /// <value>Minimum height.</value>
        public short Height_Min { get; set; }

        /// <summary>
        /// Gets or sets max height to <see cref="FileCabinetRecord"/>.
        /// </summary>
        /// Maximum height.
        public short Height_Max { get; set; }

        /// <summary>
        /// Gets or sets min amount of money to <see cref="FileCabinetRecord"/>.
        /// </summary>
        /// <value>Minimum amount of money.</value>
        public decimal Money_Min { get; set; }

        /// <summary>
        /// Gets or sets max amount of money to <see cref="FileCabinetRecord"/>.
        /// </summary>
        /// <value>Maximum amount of money.</value>
        public decimal Money_Max { get; set; }
    }
}
