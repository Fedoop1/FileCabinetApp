using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface which describes basic behavior of Input validators classes.
    /// </summary>
    public interface IInputValidator
    {
        /// <summary>
        /// Validate incoming first name using special rules.
        /// </summary>
        /// <param name="firstName">Incoming first name.</param>
        /// <returns>The result of validating.</returns>
        public bool ValidateFirstName(string firstName);

        /// <summary>
        /// Validate incoming last name using special rules.
        /// </summary>
        /// <param name="lastName">Incoming last name.</param>
        /// <returns>The result of validating.</returns>
        public bool ValidateLastName(string lastName);

        /// <summary>
        /// Validate incoming date of birth using special rules.
        /// </summary>
        /// <param name="dateOfBirth">Incoming date of birth.</param>
        /// <returns>The result of validating.</returns>
        public bool ValidateDateOfBirth(DateTime dateOfBirth);

        /// <summary>
        /// Validate incoming height using special rules.
        /// </summary>
        /// <param name="height">Incoming height.</param>
        /// <returns>The result of validating.</returns>
        public bool ValidateHeight(short height);

        /// <summary>
        /// Validate incoming amount of money using special rules.
        /// </summary>
        /// <param name="money">Incoming amount of money.</param>
        /// <returns>The result of validating.</returns>
        public bool ValidateMoney(decimal money);

        /// <summary>
        /// Validate incoming gender using special rules.
        /// </summary>
        /// <param name="gender">Incoming gender.</param>
        /// <returns>The result of validation.</returns>
        public bool ValidateGender(char gender);
    }
}
