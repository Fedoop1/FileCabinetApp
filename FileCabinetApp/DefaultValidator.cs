using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class DefaultValidator : IRecordValidator
    {
        public const short MaxHeight = short.MaxValue;
        public const short MinHeight = 10;
        public const decimal MinMoney = 0;
        public const int MinNameLength = 2;
        public const int MaxNameLength = 60;
        public static readonly char[] ValidGenderValue = { 'm', 'M', 'F', 'F' };
        public static readonly DateTime MinDateOfBirth = DateTime.Parse("1.12.1950", System.Globalization.CultureInfo.InvariantCulture);

        public void ValidateParameters(FileCabinetRecordData recordData)
        {
            if (recordData?.FirstName.Length < MinNameLength || recordData.FirstName.Length > MaxNameLength || recordData.FirstName.Any(symbol => char.IsNumber(symbol)))
            {
                throw new ArgumentException("First name is incorrect.");
            }
            else if (recordData.LastName.Length < MinNameLength || recordData.LastName.Length > MaxNameLength || recordData.FirstName.Any(symbol => char.IsNumber(symbol)))
            {
                throw new AggregateException("Last name is incorrect.");
            }
            else if (recordData.Height < MinHeight || recordData.Height > MaxHeight)
            {
                throw new ArgumentException("Height is incorrect.");
            }
            else if (recordData.Money < MinMoney)
            {
                throw new ArgumentException("Money can't be lower than zero.");
            }
            else if (!ValidGenderValue.Contains(recordData.Gender))
            {
                throw new ArgumentException("Gender is incorrect.");
            }
            else if (recordData.DateOfBirth < MinDateOfBirth || recordData.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date of birth is incorrect.");
            }
        }

    }
}
