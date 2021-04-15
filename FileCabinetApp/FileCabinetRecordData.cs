using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetRecordData : FileCabinetRecord
    {
        private short maxHeight;
        private short minHeight;
        private decimal minMoney;
        private int minNameLength;
        private int maxNameLength = 60;
        private char[] validGenderValue;
        private DateTime minDateOfBirth;

        public FileCabinetRecordData(string parameters)
        {
            switch (parameters)
            {
                case "custom":
                    this.maxHeight = CustomValidator.MaxHeight;
                    this.minHeight = CustomValidator.MinHeight;
                    this.minMoney = CustomValidator.MinMoney;
                    this.minNameLength = CustomValidator.MinNameLength;
                    this.maxNameLength = CustomValidator.MaxNameLength;
                    this.validGenderValue = CustomValidator.ValidGenderValue;
                    this.minDateOfBirth = CustomValidator.MinDateOfBirth;
                    break;
                case "default":
                    this.maxHeight = DefaultValidator.MaxHeight;
                    this.minHeight = DefaultValidator.MinHeight;
                    this.minMoney = DefaultValidator.MinMoney;
                    this.minNameLength = DefaultValidator.MinNameLength;
                    this.maxNameLength = DefaultValidator.MaxNameLength;
                    this.validGenderValue = DefaultValidator.ValidGenderValue;
                    this.minDateOfBirth = DefaultValidator.MinDateOfBirth;
                    break;
            }
        }

        public void InputData()
        {
                Console.WriteLine("\nFirst name: ");

                this.FirstName = this.ReadInput(this.StringConverter, this.StringValidator);

                Console.WriteLine("Last name: ");

                this.LastName = this.ReadInput(this.StringConverter, this.StringValidator);

                Console.WriteLine("Date of birth: ");

                this.DateOfBirth = this.ReadInput(this.DataTimeConverter, this.DataTimeValidator);

                Console.WriteLine("Height: ");

                this.Height = this.ReadInput(this.ShortConverter, this.ShortValidator);

                Console.WriteLine("Money: ");

                this.Money = this.ReadInput(this.DecimalConverter, this.DecimalValidator);

                Console.WriteLine("Gender(M/F): ");

                this.Gender = this.ReadInput(this.CharConverter, this.CharValidator);
        }

        private Tuple<bool, string> DataTimeValidator(DateTime dateOfBirth)
        {
            if (dateOfBirth < this.minDateOfBirth || dateOfBirth > DateTime.Now)
            {
                return new Tuple<bool, string>(false, dateOfBirth.ToString(Program.Culture));
            }

            return new Tuple<bool, string>(true, dateOfBirth.ToString(Program.Culture));
        }

        private Tuple<bool, string> ShortValidator(short height)
        {
            if (height < this.minHeight || height > this.maxHeight)
            {
                return new Tuple<bool, string>(false, height.ToString(Program.Culture));
            }

            return new Tuple<bool, string>(true, height.ToString(Program.Culture));
        }

        private Tuple<bool, string> DecimalValidator(decimal money)
        {
            if (money < this.minMoney)
            {
                return new Tuple<bool, string>(false, money.ToString(Program.Culture));
            }

            return new Tuple<bool, string>(true, money.ToString(Program.Culture));
        }

        private Tuple<bool, string> CharValidator(char gender)
        {
            if (!this.validGenderValue.Contains(gender))
            {
                return new Tuple<bool, string>(false, gender.ToString(Program.Culture));
            }

            return new Tuple<bool, string>(true, gender.ToString(Program.Culture));
        }

        private Tuple<bool, string, char> CharConverter(string gender)
        {
            if (!char.TryParse(gender, out char result))
            {
                return new Tuple<bool, string, char>(false, gender, default(char));
            }

            return new Tuple<bool, string, char>(true, gender, result);
        }

        private Tuple<bool, string, DateTime> DataTimeConverter(string dateOfBirth)
        {
            if (!DateTime.TryParse(dateOfBirth, out DateTime result))
            {
                return new Tuple<bool, string, DateTime>(false, dateOfBirth, default(DateTime));
            }

            return new Tuple<bool, string, DateTime>(true, dateOfBirth, result);
        }

        private Tuple<bool, string, short> ShortConverter(string height)
        {
            if (!short.TryParse(height, out short result))
            {
                return new Tuple<bool, string, short>(false, height, default(short));
            }

            return new Tuple<bool, string, short>(true, height, result);
        }

        private Tuple<bool, string, decimal> DecimalConverter(string money)
        {
            if (!decimal.TryParse(money, out decimal result))
            {
                return new Tuple<bool, string, decimal>(false, money, default(decimal));
            }

            return new Tuple<bool, string, decimal>(true, money, result);
        }

        private Tuple<bool, string> StringValidator(string name)
        {
            if (name.Length < this.minNameLength || name.Length > this.maxNameLength || name.Any(symbol => char.IsNumber(symbol)))
            {
                return new Tuple<bool, string>(false, name);
            }

            return new Tuple<bool, string>(true, name);
        }

        private Tuple<bool, string, string> StringConverter(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(name))
            {
                return new Tuple<bool, string, string>(false, name, string.Empty);
            }

            return new Tuple<bool, string, string>(true, name, name);
        }

        private T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
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
    }
}
