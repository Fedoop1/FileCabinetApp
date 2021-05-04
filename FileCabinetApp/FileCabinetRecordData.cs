using System;
using System.Linq;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// A container class for receiving and storing new information about a record.
    /// </summary>
    public class FileCabinetRecordData : FileCabinetRecord
    {
        private IInputValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordData"/> class.
        /// </summary>
        /// <param name="service">Validation rules.</param>
        public FileCabinetRecordData(IFileCabinetService service)
        {
            this.validator = service?.GetType().Name switch
            {
                "FileCabinetDefaultService" => new DefaultInputValidator(),
                "FileCabinetCustomService" => new CustomInputValidator(),
                _ => new DefaultInputValidator(),
            };
        }

        /// /// <summary>
        /// A method that collects information about a record from a user.
        /// </summary>
        public void InputData()
        {
            Console.WriteLine("\nFirst name: ");

            this.FirstName = ReadInput(this.StringConverter, this.StringValidator);

            Console.WriteLine("Last name: ");

            this.LastName = ReadInput(this.StringConverter, this.StringValidator);

            Console.WriteLine("Date of birth: ");

            this.DateOfBirth = ReadInput(this.DataTimeConverter, this.DataTimeValidator);

            Console.WriteLine("Height: ");

            this.Height = ReadInput(this.ShortConverter, this.ShortValidator);

            Console.WriteLine("Money: ");

            this.Money = ReadInput(this.DecimalConverter, this.DecimalValidator);

            Console.WriteLine("Gender(M/F): ");

            this.Gender = ReadInput(this.CharConverter, this.CharValidator);
        }

        /// <summary>
        /// Method for getting and validating input.
        /// </summary>
        /// <typeparam name="T">Generic variable denoting the type for conversion and validation.</typeparam>
        /// <param name="converter">A converter method that performs conversion according to certain rules.</param>
        /// <param name="validator">A validator method that performs validation according to certain rules.</param>
        /// <returns>Returns the entered data as T.</returns>
        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        /// <summary>
        /// The validator of the entered date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Information for verification.</param>
        /// <returns>A tuple with the result of the check and a string representation of the entered information.</returns>
        private Tuple<bool, string> DataTimeValidator(DateTime dateOfBirth)
        {
            if (this.validator.ValidateDateOfBirth(dateOfBirth))
            {
                return new Tuple<bool, string>(true, dateOfBirth.ToString(Program.Culture));
            }

            return new Tuple<bool, string>(false, dateOfBirth.ToString(Program.Culture));
        }

        /// <summary>
        /// The entered growth validator.
        /// </summary>
        /// <param name="height">Information for verification.</param>
        /// <returns>A tuple with the result of validation and a string representation of the entered information.</returns>
        private Tuple<bool, string> ShortValidator(short height)
        {
            if (this.validator.ValidateHeight(height))
            {
                return new Tuple<bool, string>(true, height.ToString(Program.Culture));
            }

            return new Tuple<bool, string>(false, height.ToString(Program.Culture));
        }

        /// <summary>
        /// Validator of the entered amount of money.
        /// </summary>
        /// <param name="money">Information for verification.</param>
        /// <returns>A tuple with the result of the check and a string representation of the entered information.</returns>
        private Tuple<bool, string> DecimalValidator(decimal money)
        {
            if (this.validator.ValidateMoney(money))
            {
                return new Tuple<bool, string>(true, money.ToString(Program.Culture));
            }

            return new Tuple<bool, string>(false, money.ToString(Program.Culture));
        }

        /// <summary>
        /// The entered gender validator.
        /// </summary>
        /// <param name="gender">Information for verification.</param>
        /// <returns>A tuple with the result of the check and a string representation of the entered information.</returns>
        private Tuple<bool, string> CharValidator(char gender)
        {
            if (this.validator.ValidateGender(gender))
            {
                return new Tuple<bool, string>(true, gender.ToString(Program.Culture));
            }

            return new Tuple<bool, string>(false, gender.ToString(Program.Culture));
        }

        /// <summary>
        /// Method for converting from String to Char.
        /// </summary>
        /// <param name="gender">Information for verification.</param>
        /// <returns>A tuple with the result of the check, a string representation of the entered information, and the converted variable.</returns>
        private Tuple<bool, string, char> CharConverter(string gender)
        {
            if (!char.TryParse(gender, out char result))
            {
                return new Tuple<bool, string, char>(false, gender, default(char));
            }

            return new Tuple<bool, string, char>(true, gender, result);
        }

        /// <summary>
        /// A method that converts from a string representation to a DateTime.
        /// </summary>
        /// <param name="dateOfBirth">Information for verification.</param>
        /// <returns>A tuple with the result of the check, a string representation of the entered information, as well as the converted variable.</returns>
        private Tuple<bool, string, DateTime> DataTimeConverter(string dateOfBirth)
        {
            if (!DateTime.TryParse(dateOfBirth, out DateTime result))
            {
                return new Tuple<bool, string, DateTime>(false, dateOfBirth, default(DateTime));
            }

            return new Tuple<bool, string, DateTime>(true, dateOfBirth, result);
        }

        /// <summary>
        /// A method that converts from String to Short.
        /// </summary>
        /// <param name="height">Information for conversion.</param>
        /// <returns>A tuple with the result of the check, a string representation of the entered information, as well as the converted variable.</returns>
        private Tuple<bool, string, short> ShortConverter(string height)
        {
            if (!short.TryParse(height, out short result))
            {
                return new Tuple<bool, string, short>(false, height, default(short));
            }

            return new Tuple<bool, string, short>(true, height, result);
        }

        /// <summary>
        /// A method that converts from String to Decimal.
        /// </summary>
        /// <param name="money">Information for conversion.</param>
        /// <returns>A tuple with the result of the check, a string representation of the entered information, as well as the converted variable.</returns>
        private Tuple<bool, string, decimal> DecimalConverter(string money)
        {
            if (!decimal.TryParse(money, out decimal result))
            {
                return new Tuple<bool, string, decimal>(false, money, default(decimal));
            }

            return new Tuple<bool, string, decimal>(true, money, result);
        }

        /// <summary>
        /// The validator of the entered first or last name.
        /// </summary>
        /// <param name="name">Information for validation.</param>
        /// <returns>A tuple with the result of the check and a string representation of the entered information.</returns>
        private Tuple<bool, string> StringValidator(string name)
        {
            if (this.validator.ValidateLastName(name))
            {
                return new Tuple<bool, string>(true, name);
            }

            return new Tuple<bool, string>(false, name);
        }

        /// <summary>
        /// A method that checks the entered string value.
        /// </summary>
        /// <param name="name">Information for verification.</param>
        /// <returns>A tuple with the result of the check, a string representation of the entered information, and the converted variable.</returns>
        private Tuple<bool, string, string> StringConverter(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(name))
            {
                return new Tuple<bool, string, string>(false, name, string.Empty);
            }

            return new Tuple<bool, string, string>(true, name, name);
        }
    }
}
