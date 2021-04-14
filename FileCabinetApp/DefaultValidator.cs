using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class DefaultValidator : IRecordValidator
    {
        protected const short MaxHeight = short.MaxValue;
        protected const short MinHeight = 10;
        protected const decimal MinMoney = 0;
        protected const int MinNameLength = 2;
        protected const int MaxNameLength = 60;
        protected static char[] validGenderValue = { 'm', 'M', 'F', 'F' };

        public void ValidateParameters(FileCabinetRecordData recordData)
        {
            if (recordData?.FirstName.Length < MinNameLength || recordData.FirstName.Length > MaxNameLength)
            {
                throw new ArgumentException("First name is incorrect.");
            }
            else if (recordData.LastName.Length < MinNameLength || recordData.LastName.Length > MaxNameLength)
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
            else if (!validGenderValue.Contains(recordData.Gender))
            {
                throw new ArgumentException("Gender is incorrect.");
            }
            else if (recordData.DateOfBirth < DateTime.Parse("1.12.1950", System.Globalization.CultureInfo.InvariantCulture) || recordData.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date of birth is incorrect.");
            }
        }

    }
}
