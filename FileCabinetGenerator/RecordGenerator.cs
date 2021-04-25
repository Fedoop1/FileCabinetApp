// <copyright file="RecordGenerator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace FileCabinetGenerator
{
    using System;
    using System.Linq;
    using FileCabinetApp;

    public static class RecordGenerator
    {
        private static Random random = new Random();

        public static FileCabinetRecord[] GenerateRecord(int startId, int recordAmount)
        {
            var resultRecordArray = new FileCabinetRecord[recordAmount];

            for (int recordCount = 0; recordCount < recordAmount; recordCount++)
            {
                int id = startId++;
                string firstName = new string(Guid.NewGuid().ToString().Where(symb => !char.IsDigit(symb) && !char.IsPunctuation(symb)).ToArray());
                string lastName = new string(Guid.NewGuid().ToString().Where(symb => !char.IsDigit(symb) && !char.IsPunctuation(symb)).ToArray());
                DateTime dateOfBirth = DateTime.Parse($"{random.Next(1, 28)}.{random.Next(1, 12)}.{random.Next(1951, DateTime.Now.Year)}", Program.Culture);
                short height = (short)random.Next(DefaultValidator.MinHeight, DefaultValidator.MaxHeight);
                decimal money = random.Next(0, int.MaxValue);
                char gender = DefaultValidator.ValidGenderValue[random.Next(0, DefaultValidator.ValidGenderValue.Length)];

                resultRecordArray[recordCount] = new FileCabinetRecord() { Id = id, FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, Height = height, Money = money, Gender = gender };
            }

            return resultRecordArray;
        }
    }
}
