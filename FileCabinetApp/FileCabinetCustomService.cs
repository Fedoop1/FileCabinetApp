using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetCustomService : FileCabinetService
    {
        protected const short MaxHeight = short.MaxValue;
        protected const short MinHeight = 55;
        protected const decimal MinMoney = 1;
        protected const int MinNameLength = 2;
        protected const int MaxNameLength = 60;
        protected static char[] validGenderValue = { 'm', 'M', 'F', 'f' };
        protected static DateTime minDateOfBirth = DateTime.Parse("01.12.1950", System.Globalization.CultureInfo.InvariantCulture);

        public override bool ValidateParameters(FileCabinetRecordData recordData)
        {
            if (recordData?.FirstName.Length < MinNameLength || recordData.FirstName.Length > MaxNameLength || recordData.FirstName.Any(symbol => char.IsNumber(symbol)))
            {
                throw new ArgumentException("First name is incorrect.");
            }
            else if (recordData.LastName.Length < MinNameLength || recordData.LastName.Length > MaxNameLength || recordData.LastName.Any(symbol => char.IsNumber(symbol)))
            {
                return false;
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
            else if (DateTime.Now.Year - recordData.DateOfBirth.Year < 18 || recordData.DateOfBirth < minDateOfBirth || recordData.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date of birth is incorrect.");
            }

            return true;
        }
    }
}

// Date of birth
