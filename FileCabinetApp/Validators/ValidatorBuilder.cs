using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    public class ValidatorBuilder
    {
        private List<IRecordValidator> validators;

        public ValidatorBuilder()
        {
            this.validators = new List<IRecordValidator>();
        }

        public ValidatorBuilder ValidateFirstName(int minFirstNameLength, int maxFirstNameLength)
        {
            this.validators.Add(new FirstNameValidator(minFirstNameLength, maxFirstNameLength));
            return this;
        }

        public ValidatorBuilder ValidateLastName(int minLastNameLength, int maxLastNameLength)
        {
            this.validators.Add(new LastNameValidator(minLastNameLength, maxLastNameLength));
            return this;
        }

        public ValidatorBuilder ValidateDateOfBirth(DateTime minDateOfBirth)
        {
            this.validators.Add(new DateOfBirthValidator(minDateOfBirth));
            return this;
        }

        public ValidatorBuilder ValidateHeight(short minHeight, short maxHeight)
        {
            this.validators.Add(new HeightValidator(minHeight, maxHeight));
            return this;
        }

        public ValidatorBuilder ValidateMoney(decimal minMoney)
        {
            this.validators.Add(new MoneyValidator(minMoney));
            return this;
        }

        public ValidatorBuilder ValidateGender(char[] validGenderArray)
        {
            this.validators.Add(new GenderValidator(validGenderArray));
            return this;
        }

        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }

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
    }
}


/*  new FirstNameValidator(2, 60),
    new LastNameValidator(2, 60),
    new DateOfBirthValidator(DateTime.Parse("1.12.1950", System.Globalization.CultureInfo.InvariantCulture)),
    new HeightValidator(10, 250),
    new MoneyValidator(0),
    new GenderValidator(new char[] { 'm', 'M', 'F', 'f' }),
*/