using System;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Interface which describe behavior of validation settings to <see cref="FileCabinetRecord"/>.
    /// </summary>
    public interface IValidationSettings
    {
        /// <summary>
        /// Get's or set's max length of first name to <see cref="FileCabinetRecord"/>.
        /// </summary>
        /// <value>Max length of first name.</value>
        public int FirstName_Max { get; set; }

        /// <summary>
        /// Get's or set's min length of first name to <see cref="FileCabinetRecord"/>.
        /// </summary>
        public int FirstName_Min { get; set; }

        /// <summary>
        /// Get's or set's max length of last name to <see cref="FileCabinetRecord"/>.
        /// </summary>
        public int LastName_Max { get; set; }

        /// <summary>
        /// Get's or set's min length of last name to <see cref="FileCabinetRecord"/>.
        /// </summary>
        public int LastName_Min { get; set; }

        /// <summary>
        /// Get's or set's minimal date of date of birth to <see cref="FileCabinetRecord"/>.
        /// </summary>
        public DateTime DateOfBirth_From { get; set; }

        /// <summary>
        /// Get's or set's maximum date of date of birth to <see cref="FileCabinetRecord"/>.
        /// </summary>
        public DateTime DateOfBirth_To { get; set; }

        /// <summary>
        /// Get's or set's min height to <see cref="FileCabinetRecord"/>.
        /// </summary>
        public short Height_Min { get; set; }

        /// <summary>
        /// Get's or set's max height to <see cref="FileCabinetRecord"/>.
        /// </summary>
        public short Height_Max { get; set; }

        /// <summary>
        /// Get's or set's min amount of money to <see cref="FileCabinetRecord"/>.
        /// </summary>
        public decimal Money_Min { get; set; }

        /// <summary>
        /// Get's or set's max amount of money to <see cref="FileCabinetRecord"/>.
        /// </summary>
        public decimal Money_Max { get; set; }
    }
}
