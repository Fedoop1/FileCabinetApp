using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    public sealed class ValidationSettings : IValidationSettings
    {
        public int FirstName_Min { get; set; }

        public int FirstName_Max { get; set; }

        public int LastName_Min { get; set; }

        public int LastName_Max { get; set; }

        public DateTime DateOfBirth_From { get; set; }

        public DateTime DateOfBirth_To { get; set; }

        public short Height_Min { get; set; }

        public short Height_Max { get; set; }

        public decimal Money_Min { get; set; }

        public decimal Money_Max { get; set; }
    }
}
