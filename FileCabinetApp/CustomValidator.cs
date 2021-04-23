using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// The class contains custom rules for records.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <summary>
        /// Максимальное значения высота роста.
        /// </summary>
        public const short MaxHeight = 250;

        /// <summary>
        /// The maximum value is the height of growth.
        /// </summary>
        public const short MinHeight = 100;

        /// <summary>
        /// The minimum amount of money.
        /// </summary>
        public const decimal MinMoney = 1;

        /// <summary>
        /// The minimum length of the first or last name.
        /// </summary>
        public const int MinNameLength = 3;

        /// <summary>
        /// The maximum length of a given name or surname.
        /// </summary>
        public const int MaxNameLength = 25;

        /// <summary>
        /// An array of valid values for gender.
        /// </summary>
        public static readonly char[] ValidGenderValue = { 'm', 'M', 'F', 'f' };

        /// <summary>
        /// The minimum allowed date of birth.
        /// </summary>
        public static readonly DateTime MinDateOfBirth = DateTime.Parse("01.12.1970", System.Globalization.CultureInfo.InvariantCulture);

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
            else if (recordData.LastName.Length < MinNameLength || recordData.LastName.Length > MaxNameLength || recordData.LastName.Any(symbol => char.IsNumber(symbol)))
            {
                throw new ArgumentException("Last name is incorrect.");
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
