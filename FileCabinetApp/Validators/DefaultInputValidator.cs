using System;
using System.Linq;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class with the default input validation rules.
    /// </summary>
    public class DefaultInputValidator : IInputValidator
    {
        /// <inheritdoc/>
        public bool ValidateDateOfBirth(DateTime dateOfBirth)
        {
            return dateOfBirth < DateTime.Now;
        }

        /// <inheritdoc/>
        public bool ValidateFirstName(string firstName)
        {
            return !(string.IsNullOrEmpty(firstName) || string.IsNullOrWhiteSpace(firstName) || firstName.Any(char.IsDigit));
        }

        /// <inheritdoc/>
        public bool ValidateGender(char gender)
        {
            return char.IsLetter(gender);
        }

        /// <inheritdoc/>
        public bool ValidateHeight(short height)
        {
            return height > 0;
        }

        /// <inheritdoc/>
        public bool ValidateLastName(string lastName)
        {
            return !(string.IsNullOrEmpty(lastName) || string.IsNullOrWhiteSpace(lastName) || lastName.Any(char.IsDigit));
        }

        /// <inheritdoc/>
        public bool ValidateMoney(decimal money)
        {
            return money > 0;
        }
    }
}
