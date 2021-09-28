using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Create validation methods and composite it to <see cref="CompositeValidator"/>.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorBuilder"/> class.
        /// </summary>
        private ValidatorBuilder()
        {
            this.validators = new List<IRecordValidator>();
        }

        /// <summary>
        /// Extension method which create <see cref="CompositeValidator"/> with the custom rules.
        /// </summary>
        /// <returns>New instance of <see cref="CompositeValidator"/> with the custom rules.</returns>
        public static IRecordValidator CreateValidator(IValidationSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings), "Settings can't be null");
            }

            return new ValidatorBuilder()
                .ValidateFirstName(settings.FirstName_Min, settings.FirstName_Max)
                .ValidateLastName(settings.LastName_Min, settings.LastName_Max)
                .ValidateDateOfBirth(settings.DateOfBirth_From, settings.DateOfBirth_To)
                .ValidateHeight(settings.Height_Min, settings.Height_Max)
                .ValidateMoney(settings.Money_Min, settings.Money_Max)
                .ValidateGender(new[] { 'f', 'F', 'M', 'm' })
                .Create();
        }

        /// <summary>
        /// Add first name validator to validators list.
        /// </summary>
        /// <param name="minFirstNameLength">The minimum length of the first name.</param>
        /// <param name="maxFirstNameLength">The maximum length of the first name.</param>
        /// <returns>Instance of validator builder with first name validator in validators list.</returns>
        private ValidatorBuilder ValidateFirstName(int minFirstNameLength, int maxFirstNameLength)
        {
            this.validators.Add(new FirstNameValidator(minFirstNameLength, maxFirstNameLength));
            return this;
        }

        /// <summary>
        /// Add last name validator to validators list.
        /// </summary>
        /// <param name="minLastNameLength">The minimum length of the last name.</param>
        /// <param name="maxLastNameLength">The maximum length of the last name.</param>
        /// <returns>Instance of validator builder with last name validator in validators list.</returns>
        private ValidatorBuilder ValidateLastName(int minLastNameLength, int maxLastNameLength)
        {
            this.validators.Add(new LastNameValidator(minLastNameLength, maxLastNameLength));
            return this;
        }

        /// <summary>
        /// Add date of birth validator to validators list.
        /// </summary>
        /// <param name="minDateOfBirth">The minimum date of birth.</param>
        /// <returns>Instance of validator builder with date of birth validator in validators list.</returns>
        private ValidatorBuilder ValidateDateOfBirth(DateTime minDateOfBirth, DateTime maxDateOfBirth)
        {
            this.validators.Add(new DateOfBirthValidator(minDateOfBirth, maxDateOfBirth));
            return this;
        }

        /// <summary>
        /// Add height validator to validators list.
        /// </summary>
        /// <param name="minHeight">The minimum available height.</param>
        /// <param name="maxHeight">The maximum available height.</param>
        /// <returns>Instance of validator builder with height validator in validators list.</returns>
        private ValidatorBuilder ValidateHeight(short minHeight, short maxHeight)
        {
            this.validators.Add(new HeightValidator(minHeight, maxHeight));
            return this;
        }

        /// <summary>
        /// Add money validator to validators list.
        /// </summary>
        /// <param name="minMoney">The minimum amount of money.</param>
        /// <returns>Instance of validator builder with money validator in validators list.</returns>
        private ValidatorBuilder ValidateMoney(decimal minMoney, decimal maxMoney)
        {
            this.validators.Add(new MoneyValidator(minMoney, maxMoney));
            return this;
        }

        /// <summary>
        /// Add gender validator to validators list.
        /// </summary>
        /// <param name="validGenderArray">Array of valid genders.</param>
        /// <returns>Instance of validator builder with gedner validator in validators list.</returns>
        private ValidatorBuilder ValidateGender(char[] validGenderArray)
        {
            this.validators.Add(new GenderValidator(validGenderArray));
            return this;
        }

        /// <summary>
        /// Create new instance of <see cref="CompositeValidator"/> with the list of validators.
        /// </summary>
        /// <returns>New instance of <see cref="CompositeValidator"/> with the list of validators.</returns>
        private IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}