using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class with input validation rules.
    /// </summary>
    public class InputValidator : IInputValidator
    {
        private readonly IValidationSettings validationSettings;

        public InputValidator(IValidationSettings validationSettings) => this.validationSettings =
            validationSettings ??
            throw new ArgumentNullException(nameof(validationSettings), "Validation settings can't be null");

        /// <inheritdoc/>
        public bool ValidateDateOfBirth(DateTime dateOfBirth) =>
            dateOfBirth > this.validationSettings.DateOfBirth_From &&
            dateOfBirth < this.validationSettings.DateOfBirth_To;

        /// <inheritdoc/>
        public bool ValidateFirstName(string firstName) => !(string.IsNullOrEmpty(firstName) &&
                                                             firstName.Length > this.validationSettings
                                                                 .FirstName_Min && firstName.Length <
                                                             this.validationSettings.FirstName_Max);

        /// <inheritdoc/>
        public bool ValidateGender(char gender) => char.IsLetter(gender) && !char.IsPunctuation(gender);

        /// <inheritdoc/>
        public bool ValidateHeight(short height) =>
            this.validationSettings.Height_Min < height && this.validationSettings.Height_Max > height;

        /// <inheritdoc/>
        public bool ValidateLastName(string lastName) => !(string.IsNullOrEmpty(lastName) &&
                                                           lastName.Length > this.validationSettings
                                                               .LastName_Min && lastName.Length <
                                                           this.validationSettings.LastName_Max);

        /// <inheritdoc/>
        public bool ValidateMoney(decimal money) =>
            this.validationSettings.Money_Min < money && this.validationSettings.Money_Max > money;
    }
}
