using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public interface IInputValidator
    {
        public bool ValidateFirstName(string firstName);

        public bool ValidateLastName(string lastName);

        public bool ValidateDateOfBirth(DateTime dateOfBirth);

        public bool ValidateHeight(short height);
        public bool ValidateMoney(decimal money);

        public bool ValidateGender(char gender);

    }
}
