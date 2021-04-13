using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetDefaultService : FileCabinetService
    {
        protected const short MaxHeight = short.MaxValue;
        protected const short MinHeight = 10;
        protected const decimal MinMoney = 0;
        protected const int MinNameLength = 2;
        protected const int MaxNameLength = 60;
        protected static char[] validGenderValue = { 'm', 'M', 'F', 'F' };

        public override bool ValidateParameters(FileCabinetRecordData recordData)
        {
            if (recordData?.FirstName.Length < MinNameLength || recordData.FirstName.Length > MaxNameLength)
            {
                return false;
            }
            else if (recordData.LastName.Length < MinNameLength || recordData.LastName.Length > MaxNameLength)
            {
                return false;
            }
            else if (recordData.Height < MinHeight || recordData.Height > MaxHeight)
            {
                return false;
            }
            else if (recordData.Money < MinMoney)
            {
                return false;
            }
            else if (!validGenderValue.Contains(recordData.Gender))
            {
                return false;
            }

            return true;
        }
    }
}
