using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// The class contains the standard rules for records.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <summary>
        /// The maximum value is the height of growth.
        /// </summary>
        public const short MaxHeight = short.MaxValue;

        /// <summary>
        /// The minimum value is the height of the growth.
        /// </summary>
        public const short MinHeight = 10;

        /// <summary>
        /// The minimum amount of money.
        /// </summary>
        public const decimal MinMoney = 0;

        /// <summary>
        /// The minimum length of the first or last name.
        /// </summary>
        public const int MinNameLength = 2;

        /// <summary>
        /// The maximum length of a given name or surname.
        /// </summary>
        public const int MaxNameLength = 60;

        /// <summary>
        /// An array of valid values for gender.
        /// </summary>
        public static readonly char[] ValidGenderValue = { 'm', 'M', 'F', 'F' };

        /// <summary>
        /// The minimum allowed date of birth.
        /// </summary>
        public static readonly DateTime MinDateOfBirth = DateTime.Parse("1.12.1950", System.Globalization.CultureInfo.InvariantCulture);

        /// <summary>
        /// A method that checks values according to established rules.
        /// </summary>
        /// <param name="recordData">The class "container" with information about the record.</param>
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
