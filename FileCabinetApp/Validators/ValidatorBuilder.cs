using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ValidatorBuilder()
        {
            this.validators = new List<IRecordValidator>();
        }

        /// <summary>
        /// Extension method which create <see cref="CompositeValidator"/> with the default rules.
        /// </summary>
        /// <returns>New instance of <see cref="CompositeValidator"/> with the default rules.</returns>
        public static IRecordValidator CreateDefault()
        {
            return new ValidatorBuilder()
                .ValidateFirstName(2, 60)
                .ValidateLastName(2, 60)
                .ValidateDateOfBirth(DateTime.Parse("1.12.1950", System.Globalization.CultureInfo.InvariantCulture))
                .ValidateHeight(10, short.MaxValue)
                .ValidateMoney(0)
                .ValidateGender(new char[] { 'f', 'F', 'M', 'm' })
                .Create();
        }

        /// <summary>
        /// Extension method which create <see cref="CompositeValidator"/> with the custom rules.
        /// </summary>
        /// <returns>New instance of <see cref="CompositeValidator"/> with the custom rules.</returns>
        public static IRecordValidator CreateCustom()
        {
            return new ValidatorBuilder()
                .ValidateFirstName(4, 60)
                .ValidateLastName(4, 60)
                .ValidateDateOfBirth(DateTime.Parse("1.12.1970", System.Globalization.CultureInfo.InvariantCulture))
                .ValidateHeight(50, 250)
                .ValidateMoney(100)
                .ValidateGender(new char[] { 'f', 'F', 'M', 'm' })
                .Create();
        }

        /// <summary>
        /// Add first name validator to validators list.
        /// </summary>
        /// <param name="minFirstNameLength">The minimum length of the first name.</param>
        /// <param name="maxFirstNameLength">The maximum length of the first name.</param>
        /// <returns>Instance of validator builder with first name validator in validators list.</returns>
        public ValidatorBuilder ValidateFirstName(int minFirstNameLength, int maxFirstNameLength)
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
        public ValidatorBuilder ValidateLastName(int minLastNameLength, int maxLastNameLength)
        {
            this.validators.Add(new LastNameValidator(minLastNameLength, maxLastNameLength));
            return this;
        }

        /// <summary>
        /// Add date of birth validator to validators list.
        /// </summary>
        /// <param name="minDateOfBirth">The minimum date of birth.</param>
        /// <returns>Instance of validator builder with date of birth validator in validators list.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime minDateOfBirth)
        {
            this.validators.Add(new DateOfBirthValidator(minDateOfBirth));
            return this;
        }

        /// <summary>
        /// Add height validator to validators list.
        /// </summary>
        /// <param name="minHeight">The minimum available height.</param>
        /// <param name="maxHeight">The maximum available height.</param>
        /// <returns>Instance of validator builder with height validator in validators list.</returns>
        public ValidatorBuilder ValidateHeight(short minHeight, short maxHeight)
        {
            this.validators.Add(new HeightValidator(minHeight, maxHeight));
            return this;
        }

        /// <summary>
        /// Add money validator to validators list.
        /// </summary>
        /// <param name="minMoney">The minimum amount of money.</param>
        /// <returns>Instance of validator builder with money validator in validators list.</returns>
        public ValidatorBuilder ValidateMoney(decimal minMoney)
        {
            this.validators.Add(new MoneyValidator(minMoney));
            return this;
        }

        /// <summary>
        /// Add gender validator to validators list.
        /// </summary>
        /// <param name="validGenderArray">Array of valid genders.</param>
        /// <returns>Instance of validator builder with gedner validator in validators list.</returns>
        public ValidatorBuilder ValidateGender(char[] validGenderArray)
        {
            this.validators.Add(new GenderValidator(validGenderArray));
            return this;
        }

        /// <summary>
        /// Create new instance of <see cref="CompositeValidator"/> with the list of validators.
        /// </summary>
        /// <returns>New instance of <see cref="CompositeValidator"/> with the list of validators.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}