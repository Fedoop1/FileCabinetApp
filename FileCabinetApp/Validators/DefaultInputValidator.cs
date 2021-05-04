using System;
using System.Linq;

namespace FileCabinetApp
{
    public class DefaultInputValidator : IInputValidator
    {
        public bool ValidateDateOfBirth(DateTime dateOfBirth)
        {
            return dateOfBirth < DateTime.Now;
        }

        public bool ValidateFirstName(string firstName)
        {
            return !(string.IsNullOrEmpty(firstName) || string.IsNullOrWhiteSpace(firstName) || firstName.Any(symb => char.IsDigit(symb)));
        }

        public bool ValidateGender(char gender)
        {
            return char.IsLetter(gender);
        }

        public bool ValidateHeight(short height)
        {
            return height > 0;
        }

        public bool ValidateLastName(string lastName)
        {
            return !(string.IsNullOrEmpty(lastName) || string.IsNullOrWhiteSpace(lastName) || lastName.Any(symb => char.IsDigit(symb)));
        }

        public bool ValidateMoney(decimal money)
        {
            return money > 0;
        }
    }
}
