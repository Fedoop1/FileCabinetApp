using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class CustomInputValidator : IInputValidator
    {
        public bool ValidateDateOfBirth(DateTime dateOfBirth)
        {
            return dateOfBirth < DateTime.Now && DateTime.Now.Year - dateOfBirth.Year >= 18;
        }

        public bool ValidateFirstName(string firstName)
        {
            return !(string.IsNullOrEmpty(firstName) || string.IsNullOrWhiteSpace(firstName) || firstName.Any(symb => char.IsDigit(symb)));
        }

        public bool ValidateGender(char gender)
        {
            return char.IsLetter(gender) && !char.IsPunctuation(gender);
        }

        public bool ValidateHeight(short height)
        {
            return height > 50;
        }

        public bool ValidateLastName(string lastName)
        {
            return !(string.IsNullOrEmpty(lastName) || string.IsNullOrWhiteSpace(lastName) || lastName.Any(symb => char.IsDigit(symb)));
        }

        public bool ValidateMoney(decimal money)
        {
            return money > 1;
        }
    }
}
